namespace Services.OrderAPI.Application.Dtos;

public class OrderDto
{
    public Guid? Id { get; set; }

    public Guid CustomerId { get; set; } = new Guid();

    public Guid RestaurantId { get; set; } = new Guid();

    public Guid TrackingId { get; set; } = new Guid();

    public decimal Price { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string? FailureMessages { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public IEnumerable<OrderItemDto> Items { get; set; }
}
