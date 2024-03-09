using Items.Models.Commands;

namespace Items.Abstractions.Commands.Handlers
{
    public interface IProcessCreatedPaymentsCommandHandler
        : ICommandHandler<ProcessCreatedPaymentsCommand>
    {

    }
}
