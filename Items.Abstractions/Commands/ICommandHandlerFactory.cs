namespace Items.Abstractions.Commands
{
    public interface ICommandHandlerFactory<THandlerInterface>
    {
        THandlerInterface CreateHandler();
    }
}
