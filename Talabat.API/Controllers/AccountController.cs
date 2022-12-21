using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.API.Extensions;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Identity;

namespace Talabat.API.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailAsync(registerDto.Email).Result.Value)
                new BadRequestObjectResult(new ApiValidationErrorResponse() { Errors = new[] { "Email Address already in use" } });
            var user = new AppUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.Email.Split("@")[0],
                DisplayName = registerDto.DisplayName,
                PhoneNumber = registerDto.PhoneNumber,
                Address = new Address()
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    City = registerDto.City,
                    Country = registerDto.Country,
                    Street = registerDto.Street,
                    ZipCode = registerDto.ZipCode
                }
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));

            var userDto = new UserDto()
            {
                Email = registerDto.Email,
                DisplayName = $"{user.DisplayName}",
                Token = await _tokenService.CreateToken(user, _userManager)
            };

            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user is null)
                return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!result.Succeeded)
                return Unauthorized(new ApiResponse(401));

            var userDto = new UserDto()
            {
                Email = loginDto.Email,
                DisplayName = $"{user.DisplayName}",
                Token = await _tokenService.CreateToken(user, _userManager)
            };

            return Ok(userDto);

        }

        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            return Ok(new UserDto() { Email = user.Email, DisplayName = $"{user.DisplayName}", 
                Token = await _tokenService.CreateToken(user, _userManager) });
        }

        [Authorize]
        [HttpGet("GetUserAddress")]
        public async Task<ActionResult<UserDto>> GetUserAddress()
        {
            var user = await _userManager.FindByEmailWithAddressAsync(User);
            var usermapped = _mapper.Map<Address, AddressDto>(user.Address);
            return Ok(usermapped);
        }
        
        [Authorize]
        [HttpPut("UpdateUserAddress")]
        public async Task<ActionResult<UserDto>> UpdateUserAddress(AddressDto address)
        {
            var user = await _userManager.FindByEmailWithAddressAsync(User);
            
            user.Address = _mapper.Map<AddressDto, Address>(address);

            var result = await _userManager.UpdateAsync(user);

            if(result.Succeeded)
                return Ok(_mapper.Map<Address, AddressDto>(user.Address));

            return BadRequest(new ApiResponse(400, "A problem occured while updating adddress!"));
        }



        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailAsync([FromQuery] string email)
            => await _userManager.FindByEmailAsync(email) != null;


    }
}
