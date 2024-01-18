namespace Items.Abstractions.Queries.Common
{
    public interface IQueryHandler<TQuery, TResponse>
        where TResponse : class
    {
        Task<TResponse> ExecuteAsync(TQuery query, CancellationToken cancellationToken);
    }
}
