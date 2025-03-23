using System.Net;

namespace Chowbro.Core
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; }
        public string Message { get; }
        public T? Data { get; }
        public List<string>? Errors { get; }

        private ApiResponse(HttpStatusCode statusCode, string message, T? data = default, List<string>? errors = null)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public static ApiResponse<T> Success(T? data = default, string message = "Request successful", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ApiResponse<T>(statusCode, message, data);
        }

        public static ApiResponse<T> Fail(T? data, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, List<string>? errors = null)
        {
            return new ApiResponse<T>(statusCode, message, default, errors);
        }
        
       
    }
}