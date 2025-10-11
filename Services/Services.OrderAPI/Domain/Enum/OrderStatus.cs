using System.Runtime.Serialization;

namespace Services.OrderAPI.Domain.Enum;

public enum OrderStatus
{

    [EnumMember(Value = "PENDING")]
    Pending,

    [EnumMember(Value = "PAID")]
    Paid,

    [EnumMember(Value = "APPROVED")]
    Approved,

    [EnumMember(Value = "CANCELLING")]
    Cancelling,

    [EnumMember(Value = "CANCELLED")]
    Cancelled
}