using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Common.Mediator;

namespace UserManagement.Infrastructure.Mediator;

public sealed class Mediator : ISender
{
    private readonly IServiceScopeFactory _scopeFactory;

    public Mediator(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = sp.GetRequiredService(handlerType);

        var method = handlerType.GetMethod("Handle")!;
        var task = (Task<TResponse>)method.Invoke(handler, new object[] { request, cancellationToken })!;
        return await task.ConfigureAwait(false);
    }
}