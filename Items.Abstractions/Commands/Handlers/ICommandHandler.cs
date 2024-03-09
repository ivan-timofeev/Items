namespace Items.Abstractions.Commands.Handlers
{

    public interface ICommandHandler<TCommand>
    {
        Task ExecuteAsync(
            TCommand command,
            CancellationToken cancellationToken);
    }
}
