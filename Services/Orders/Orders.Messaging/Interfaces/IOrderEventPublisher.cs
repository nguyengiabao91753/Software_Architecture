using Integrations.Messaging.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Messaging.Interfaces;
public interface IOrderEventPublisher
{
    Task PublishOrderPlacedAsync(OrderPlacedEvent model);
}
