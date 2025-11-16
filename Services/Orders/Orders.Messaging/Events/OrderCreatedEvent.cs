namespace Orders.Messaging.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    string VoucherCode,
    int QuantityUsed
);
