using Items.Abstractions.Queries.Common;

namespace Items.Abstractions.Queries
{
    public interface IQueryHandlerFactory<TQueryHandler, TQuery, TResponse>
        where TQueryHandler : IQueryHandler<TQuery, TResponse>
        where TResponse : class
    {
        TQueryHandler CreateHandler();
        TQueryHandler CreateCachedHandler();
    }
}
