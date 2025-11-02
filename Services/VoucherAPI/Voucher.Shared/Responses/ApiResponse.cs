namespace Voucher.Shared.Responses;

/// <summary>
/// Mẫu response thống nhất cho toàn bộ API.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Message = message ?? "Success", Data = data };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message };
}
