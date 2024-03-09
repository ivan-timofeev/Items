namespace Items.Models.DataTransferObjects.Order
{
    public sealed class ProcessCreatedPaymentsCommand
    {
        public required int TakeLimit { get; init; }
        public required int MaxDegreeOfParallelism { get; init; }
    }
}
