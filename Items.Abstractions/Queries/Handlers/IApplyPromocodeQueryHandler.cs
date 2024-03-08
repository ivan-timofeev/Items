using Items.Abstractions.Queries.Common;
using Items.Models.DataTransferObjects;
using Items.Models.Queries;

namespace Items.Abstractions.Queries.Handlers
{
    public interface IApplyPromocodeQueryHandler
        : IQueryHandler<ApplyPromocodeQuery, ApplyPromocodeResponse>
    {

    }
}
