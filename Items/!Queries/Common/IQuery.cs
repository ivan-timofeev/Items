namespace Items.Queries
{
    public interface IQuery<T>
    {
        Task<T> ExecuteAsync(CancellationToken cancellationToken);
    }
}
