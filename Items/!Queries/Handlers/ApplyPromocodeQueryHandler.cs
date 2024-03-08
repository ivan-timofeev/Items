using Items.Abstractions.Queries.Handlers;
using Items.Data;
using Items.Models.DataTransferObjects;
using Items.Models.Queries;

namespace Items.Queries.Handlers
{
    internal sealed class ApplyPromocodeQueryHandler : IApplyPromocodeQueryHandler
    {
        private readonly DbContextProvider _dbContextProvider;

        public ApplyPromocodeQueryHandler(DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<ApplyPromocodeResponse> ExecuteAsync(
            ApplyPromocodeQuery query,
            CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);

            var response = new ApplyPromocodeResponse
            {
                CartItems = query.CartItems.Select(ci => new ApplyPromocodeResponse.CartItem
                {
                    ItemId = ci.ItemId,
                    NewPrice = 1,
                    DiscountPercentage = 0.99M,
                    OldPrice = dbContext
                        .Items
                        .Where(i => i.Id == ci.ItemId)
                        .Single()
                        .Price
                })
            };

            return response;
        }
    }
}
