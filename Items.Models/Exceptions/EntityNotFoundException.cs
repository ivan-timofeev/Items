namespace Items.Models.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string typeName, string id)
            : base($"{typeName} with provided id not found.")
        {
            Data.Add("Id", id);
        }
    }
}
