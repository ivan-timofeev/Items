using Items.Models.DataTransferObjects.Item;

namespace Items.Commands
{
    public interface ICommandsFactory
    {
        public ICommand CreateUpdateItemCommand(Guid itemId, ItemDto itemDto);
        public ICommand CreateEnsureIsDatabaseAliveCommand();
    }
}
