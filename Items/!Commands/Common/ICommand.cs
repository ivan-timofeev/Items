namespace Items.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
