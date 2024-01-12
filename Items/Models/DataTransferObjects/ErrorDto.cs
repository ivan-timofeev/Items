using System.Collections;

namespace Items.Models.DataTransferObjects
{
    public class ErrorDto
    {
        public required string ErrorMessage { get; init; }
        public IDictionary? Data { get; init; }
    }
}
