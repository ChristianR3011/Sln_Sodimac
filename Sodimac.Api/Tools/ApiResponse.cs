namespace Sodimac.Api.Tools
{
    public class ApiResponse<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string? Message { get; private set; }
        public List<ApiError>? Errors { get; private set; }

        public static ApiResponse<T> Ok(T data, string? message = null)
        {
            return new() { Success = true, Data = data, Message = message };
        }

        public static ApiResponse<T> Fail(string message, List<ApiError>? errors = null)
        {
            return new() { Success = false, Data = default(T) , Message = message, Errors = errors };
        }
    }

    public class ApiError
    {
        public string Code { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string? Field { get; set; }
    }
}
