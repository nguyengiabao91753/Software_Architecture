namespace Voucher.Shared.Requests;

/// <summary>
/// Request body để cập nhật trạng thái voucher.
/// Dùng chung giữa CommandAPI và QueryAPI.
/// </summary>
public record UpdateStatusRequest(string Status);
