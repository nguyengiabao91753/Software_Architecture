using FluentValidation;
using Services.OrderAPI.Application.Dtos;

namespace Services.OrderAPI.Application.Validators;

public class OrderItemDtoValidator:AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Item price must be greater than zero.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Item quantity must be greater than zero.");
        RuleFor(x => x.SubTotal)
            .GreaterThan(0).WithMessage("Item subtotal must be greater than zero.");
        RuleFor(x => x)
            .Must(ValidateSubTotal)
            .WithMessage("Item subtotal does not match price multiplied by quantity.");
    }
    private bool ValidateSubTotal(OrderItemDto item)
    {
        var calculated = item.Price * item.Quantity;
        return Math.Round(item.SubTotal, 2) == Math.Round(calculated, 2);
    }
}
