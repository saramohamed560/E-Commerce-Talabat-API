namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int statusCode, string? message=null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);

        }

        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A Bad Requst , You Have Made ",
                401 => "You Are Not Authorized",
                404 => "Resource Not Found",
                500 => "Server Error",
                _ => null
            } ;
        }
    }
}
