namespace Items.Abstractions.Queries
{
    public interface IQueryHandlerFactory<THandlerInterface>
    {
        THandlerInterface CreateHandler();
    }
}
