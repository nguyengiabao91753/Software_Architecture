namespace Services.OrderAPI.Application.Dtos;

public class OrderItemDto
{
    public Guid? Id { get; set; }

    public Guid? OrderId { get; set; }

    public Guid ProductId { get; set; } = new Guid();

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal { get; set; }
}
