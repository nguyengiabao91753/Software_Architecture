using FluentValidation;
using Services.OrderAPI.Application.Dtos;

namespace Services.OrderAPI.Application.Validators;

public class OrderDtoValidator: AbstractValidator<OrderDto>
{
    public OrderDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("RestaurantId is required.");

        RuleFor(x => x.TrackingId)
            .NotEmpty().WithMessage("TrackingId is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Order total price must be greater than zero.");

        RuleFor(x => x.OrderStatus)
            .NotEmpty().WithMessage("Order status is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item.");

      
        RuleForEach(x => x.Items).SetValidator(new OrderItemDtoValidator());

    
        RuleFor(x => x)
            .Must(ValidateTotalPrice)
            .WithMessage("Order total does not match the sum of item subtotals.");
    }

    private bool ValidateTotalPrice(OrderDto order)
    {
        if (order.Items == null || !order.Items.Any())
            return true;

        var calculated = order.Items.Sum(i => i.SubTotal);
        return Math.Round(order.Price, 2) == Math.Round(calculated, 2);
    }
}
