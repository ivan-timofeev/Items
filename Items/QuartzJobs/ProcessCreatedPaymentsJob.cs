using Items.Abstractions.Commands.Factories;
using Items.Abstractions.Commands.Handlers;
using Items.Commands.Handlers;
using Items.Models.DataTransferObjects.Order;
using Quartz;
using Serilog;

namespace Items.QuartzJobs
{
    [DisallowConcurrentExecution]
    internal sealed class ProcessCreatedPaymentsJob : IJob
    {
        private readonly ICommandHandlerFactory<IProcessCreatedPaymentsCommandHandler> _commandHandlerFactory;

        public ProcessCreatedPaymentsJob(ICommandHandlerFactory<IProcessCreatedPaymentsCommandHandler> commandHandlerFactory)
        {
            _commandHandlerFactory = commandHandlerFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var command = new ProcessCreatedPaymentsCommand
            {
                MaxDegreeOfParallelism = 8,
                TakeLimit = 100
            };

            await _commandHandlerFactory
                .CreateHandler()
                .ExecuteAsync(command, context.CancellationToken);
        }
    }
}
