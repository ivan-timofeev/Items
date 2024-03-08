using Items.Abstractions.Queries.Handlers;
using Items.Models.DataTransferObjects;
using Items.Models.Queries;

namespace Items.Abstractions.Queries.Factories
{
    public interface IApplyPromocodeQueryHandlerFactory
        : IQueryHandlerFactory<IApplyPromocodeQueryHandler, ApplyPromocodeQuery, ApplyPromocodeResponse>
    {

    }
}
