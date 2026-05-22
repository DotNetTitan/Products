namespace Products.Application.Abstractions;

public interface ICommandHandler<TCommand, TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}
