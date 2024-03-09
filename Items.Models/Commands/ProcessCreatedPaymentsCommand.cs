#nullable disable

using System.ComponentModel.DataAnnotations;

namespace Items.Models.Commands
{
    public sealed class ProcessCreatedPaymentsCommand
    {
        [Required]
        public int TakeLimit { get; init; }

        [Required]
        public int MaxDegreeOfParallelism { get; init; }
    }
}
