using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.ValueObjects;
public record RestaurantId
{
    public Guid Value { get; }
    private RestaurantId(Guid value) => Value = value;
    public static RestaurantId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new ArgumentException("RestaurantId cannot be empty.");
        }
        return new RestaurantId(value);
    }
}
