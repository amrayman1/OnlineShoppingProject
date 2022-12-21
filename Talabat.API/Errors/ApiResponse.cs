namespace Talabat.API.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        private string GetDefaultMessageForStatusCode(int statusCode)
            => statusCode switch
            {
                400 => "A bad request, you have made",
                401 => "you are not Authorized",
                404 => "Resourse not found",
                500 => "Errors are the path to the dark side",
                _ => null
            };

    }
}
