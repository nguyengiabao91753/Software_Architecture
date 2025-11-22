using Integrations.Messaging.Events;
using MassTransit;

namespace Orders.Orchestrator;

public class OrderOrchestrationSaga : MassTransitStateMachine<OrderStateData>
{
    

    public State OrderSubmited { get; private set; } = null!;
    public State AwaitingPayment { get; private set; } = null!;
    public State AwaitingVoucher { get; private set; } = null!;

    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;


    //Schedules
    //public Schedule<OrderStateData, PaymentTimeout> PaymentTimeout { get; private set; }
    //public Schedule<OrderStateData, VoucherTimeout> VoucherTimeout { get; private set; }

    // Events
    public Event<OrderPlacedEvent> OrderPlaced { get; private set; } = null!;
    public Event<PaymentSuceessded> PaymentSucceeded { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailed { get; private set; } = null!;
    public Event<VoucherApplied> VoucherApplied { get; private set; } = null!;
    public Event<VoucherApplyFailed> VoucherApplyFailed { get; private set; } = null!;




    public OrderOrchestrationSaga()
    {
        InstanceState(x => x.CurrentState);
        Event(() => OrderPlaced, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentSucceeded, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentFailed, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => VoucherApplied, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => VoucherApplyFailed, x => x.CorrelateById(context => context.Message.OrderId));

        //Schedules
        //Schedule(() => PaymentTimeout, x => x.PaymentTimeoutTokenId);
        //Schedule(() => VoucherTimeout, x => x.VoucherTimeoutTokenId);

        Initially(
            When(OrderPlaced)
                .Then(ctx =>
                {
                    ctx.Saga.CustomerId = ctx.Message.CustomerId;
                    ctx.Saga.RestaurantId = ctx.Message.RestaurantId;
                    ctx.Saga.TrackingId = ctx.Message.TrackingId;
                    ctx.Saga.VoucherId = ctx.Message.VoucherId;
                    ctx.Saga.TotalAmount = ctx.Message.TotalAmount;
                    ctx.Saga.OrderItems = ctx.Message.OrderItems;
                })
                .TransitionTo(AwaitingPayment)
                //.Schedule(PaymentTimeout, ctx => new PaymentTimeout(ctx.Saga.CorrelationId),
                //   ctx => TimeSpan.FromMinutes(2)
                //)
        );

        During(AwaitingPayment,
            When(PaymentSucceeded)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentProcessed = true;
                })
                .If(x => x.Saga.VoucherId == null,
                    then => then
                        .TransitionTo(Completed)
                        .Finalize())
                .If(x => x.Saga.VoucherId != null,
                    then => then
                        .TransitionTo(AwaitingVoucher)
                         //.Schedule(VoucherTimeout, ctx => new VoucherTimeout(ctx.Saga.CorrelationId),
                         //      ctx => TimeSpan.FromMinutes(2)
                         //   )
                        .Publish(ctx => new ApplyVoucherCommand(ctx.Saga.CorrelationId, ctx.Saga.VoucherId!.Value))
                ),
            When(PaymentFailed)
             .Then(ctx =>
                 {
                     ctx.Saga.PaymentProcessed = false;
                 })
                .Publish(ctx => new PaymentFailed(ctx.Saga.CorrelationId, "Payment Process Failed!"))
                .Publish(ctx => new OrderCancelled(ctx.Saga.CorrelationId))
                .TransitionTo(Failed)
                .Finalize()

            //When(PaymentTimeout.Received)
            //    .Publish(ctx => new PaymentFailed(ctx.Saga.CorrelationId, "Payment Timeout!"))
            //    .Publish(ctx => new OrderCancelled(ctx.Saga.CorrelationId))
            //    .TransitionTo(Failed)
            //    .Finalize()
        );

        During(AwaitingVoucher,
            When(VoucherApplied)
                .Then(x => x.Saga.VoucherApplied = true)
                .TransitionTo(Completed)
                .Finalize(),
            When(VoucherApplyFailed)
                .Then(x => x.Saga.VoucherApplied = false)
                .Publish(ctx => new VoucherApplyFailed(ctx.Saga.CorrelationId, "Voucher Apply Failed!"))
                .Publish(ctx => new OrderCancelled(ctx.Saga.CorrelationId))
                .TransitionTo(Failed)
                .Finalize()
            //    ,
            //When(VoucherTimeout.Received)
            //    .Publish(ctx => new VoucherApplyFailed(ctx.Saga.CorrelationId, "Voucher Apply Timeout!"))
            //    .Publish(ctx => new OrderCancelled(ctx.Saga.CorrelationId))
            //    .TransitionTo(Failed)
            //    .Finalize() 
        );
    }
}
