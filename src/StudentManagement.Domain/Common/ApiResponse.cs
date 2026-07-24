namespace StudentManagement.Domain.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, string message, T? data, int statusCode, List<string>? errors = null)
    {
        Success = success;
        Message = message;
        Data = data;
        StatusCode = statusCode;
        Errors = errors ?? new List<string>();
    }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = 200)
    {
        return new ApiResponse<T>(true, message, data, statusCode);
    }

    public static ApiResponse<T> FailureResponse(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse<T>(false, message, default, statusCode, errors);
    }
}
