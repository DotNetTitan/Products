namespace Products.Application.Abstractions;

public interface IQueryHandler<TQuery, TResponse>
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
