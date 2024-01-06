namespace Items.Models
{
    public class ItemCategory
    {
        public Guid Id { get; set; }
        public required string DisplayName { get; set; }
        public IList<Item> Items { get; set; }

        public ItemCategory()
        {
            Items = new List<Item>();
        }
    }
}
