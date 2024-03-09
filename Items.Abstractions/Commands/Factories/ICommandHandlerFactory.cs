using Items.Abstractions.Commands.Handlers;
using Items.Models.DataTransferObjects.Order;
using System.Reflection.Metadata;

namespace Items.Abstractions.Commands.Factories
{
    public interface ICommandHandlerFactory<THandlerInterface>
    {
        THandlerInterface CreateHandler();
    }
}
