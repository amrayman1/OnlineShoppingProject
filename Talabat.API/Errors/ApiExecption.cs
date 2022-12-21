namespace Talabat.API.Errors
{
    public class ApiExecption : ApiResponse
    {
        public string Details { get; set; }
        public ApiExecption(int statusCode, string message = null, string details = null) 
            : base(statusCode, message)
        {
            Details = details;
        }
    }
}
