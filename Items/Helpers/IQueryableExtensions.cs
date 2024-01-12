using Items.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Items.Helpers
{
    public static class IQueryableExtensions
    {
        public static async Task<T> SingleOrThrowNotFoundExceptionAsync<T>(
            this IQueryable<T> queryable,
            string typeName,
            string id,
            CancellationToken cancellationToken)
        {
            return await queryable.SingleOrDefaultAsync(cancellationToken) ??
                throw new EntityNotFoundException(typeName, id);
        }
    }
}
