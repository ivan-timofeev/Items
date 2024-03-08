using Items.Abstractions.Commands.Handlers;

namespace Items.Abstractions.Commands.Factories
{
    public interface ICreateOrderCommandHandlerFactory
    {
        ICreateOrderCommandHandler CreateHandler();
    }
}
