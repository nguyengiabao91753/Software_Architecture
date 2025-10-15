using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.ValueObjects;
public record TrackingId
{
    public Guid Value { get; }
    private TrackingId(Guid value) => Value = value;
    public static TrackingId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new ArgumentException("TrackingId cannot be empty.");
        }
        return new TrackingId(value);
    }
}
