using Dapper;
using MediatR;
using YazilimAcademy.Application.Common.Interfaces;
using YazilimAcademy.Application.Common.Models.Pagination;

namespace YazilimAcademy.Application.Features.Categories.Queries.GetAll;

public sealed class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PaginatedList<GetAllCategoriesDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetAllCategoriesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<PaginatedList<GetAllCategoriesDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var connection = _sqlConnectionFactory.CreateConnection();

        var offset = (request.PageNumber - 1) * request.PageSize;
        var pageSize = request.PageSize;

        var totalCount = await connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM categories");

        var categories = (await connection.QueryAsync<GetAllCategoriesDto>(
            "SELECT Id, Name FROM categories ORDER BY Name OFFSET @Offset LIMIT @PageSize",
            new { Offset = offset, PageSize = pageSize }
        )).ToList();

        return new PaginatedList<GetAllCategoriesDto>(categories, totalCount, request.PageNumber, request.PageSize);
    }
}
