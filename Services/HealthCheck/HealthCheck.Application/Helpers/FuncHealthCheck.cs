using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.Application.Helpers;

public class FuncHealthCheck : IHealthCheck
{
    private readonly Func<CancellationToken, Task<HealthCheckResult>> _check;

    public FuncHealthCheck(Func<CancellationToken, Task<HealthCheckResult>> check)
    {
        _check = check;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return _check(cancellationToken);
    }
}
