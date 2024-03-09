#nullable disable
using Items.Models.DataTransferObjects.Item;
using System.ComponentModel.DataAnnotations;

namespace Items.Models.Commands
{
    public sealed class UpdateItemCommand
    {
        [Required]
        public Guid ItemId { get; init; }

        [Required]
        public ItemDto ItemDto { get; init; }
    }
}
