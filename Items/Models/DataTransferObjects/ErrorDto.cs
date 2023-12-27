namespace Items.Models.DataTransferObjects
{
    public class ErrorDto
    {
        public required string ErrorMessage { get; init; }
        public string? Details { get; init; }
    }
}
