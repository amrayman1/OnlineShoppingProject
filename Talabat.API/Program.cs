using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.API.Extensions;
using Talabat.API.Helper;
using Talabat.API.Middlewares;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Repositories;
using Talabat.DAL.Data;
using Talabat.DAL.Entities.Data;
using Talabat.DAL.Identity;

namespace Talabat.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                var connnection = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(connnection);
            });

            builder.Services.AddApplicationServices();

            builder.Services.AddIdentityServices(builder.Configuration);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerDocumentation();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();
            
            SeedData();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();

            async void SeedData()
            {
                using(var scope = app.Services.CreateAsyncScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                    try
                    {
                        var context = serviceProvider.GetRequiredService<StoreContext>();
                        await context.Database.MigrateAsync();

                        await StoreContextSeed.SeedAsync(context, loggerFactory);

                        var identityContext = serviceProvider.GetRequiredService<AppIdentityDbContext>();
                        await identityContext.Database.MigrateAsync();

                        var userManger = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                        await AppIdentityDbContextSeed.SeedUserAsync(userManger);

                    }
                    catch (Exception ex)
                    {
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogError(ex.Message);
                        
                    }
                }
            }

        }
    }
}