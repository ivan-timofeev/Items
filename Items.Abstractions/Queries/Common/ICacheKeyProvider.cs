namespace Items.Abstractions.Queries.Common
{
    public interface ICacheKeyProvider<TQuery>
        where TQuery : class
    {
        string GetCacheKey(TQuery query);
    }
}
