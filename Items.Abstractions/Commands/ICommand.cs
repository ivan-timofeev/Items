namespace Items.Abstractions.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
