using Integrations.Messaging.Events;
using MassTransit;

namespace Orders.Orchestrator;

public class OrderStateData : SagaStateMachineInstance, ISaga, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public Guid CustomerId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid TrackingId { get; set; }
    public Guid? VoucherId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemEvent> OrderItems { get; set; } = new();
    

    public bool PaymentProcessed { get; set; }
    
    public bool? VoucherApplied { get; set; } = null;


    public Guid? PaymentTimeoutTokenId { get; set; }   // token để hủy timeout payment
    public Guid? VoucherTimeoutTokenId { get; set; }


    // ISagaVersion implementation
    public int Version { get; set; }
}
