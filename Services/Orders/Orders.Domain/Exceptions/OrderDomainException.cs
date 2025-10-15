using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.Exceptions;
public class OrderDomainException : Exception
{
    public OrderDomainException(string message)
        : base($"Domain Exception: \"{message}\" throws from Domain Layer.")
    {
    }
}
