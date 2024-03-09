using Items.Abstractions.Commands;
using Items.Abstractions.Commands.Handlers;
using Items.Models.Commands;
using Quartz;

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
