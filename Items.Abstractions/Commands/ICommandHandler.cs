namespace Items.Abstractions.Commands
{
    public interface ICommandHandler<TCommand>
    {
        Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
    }
}
