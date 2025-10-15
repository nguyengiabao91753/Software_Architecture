using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.ValueObjects;
public record VoucherId
{
    public Guid Value { get; }
    private VoucherId(Guid value) => Value = value;
    public static VoucherId Of(Guid value)
    {
        return new VoucherId(value);
    }
}
