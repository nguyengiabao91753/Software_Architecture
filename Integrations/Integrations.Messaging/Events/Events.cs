using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrations.Messaging.Events
{
    public record ProcessPaymentCommand(
     Guid OrderId,
     Guid CustomerId,
     decimal Amount
    );
    public record PaymentSuceessded(Guid OrderId);
    public record PaymentFailed(Guid OrderId, string Message);

    public record ApplyVoucherCommand(
        Guid OrderId,
        Guid? VoucherId
    );
    public record VoucherApplied(Guid OrderId);
    public record VoucherApplyFailed(Guid OrderId, string Message);

    public record OrderCancelled(Guid OrderId);



    //Schedule
    public record PaymentTimeout(Guid OrderId);
    public record VoucherTimeout(Guid OrderId);
}
