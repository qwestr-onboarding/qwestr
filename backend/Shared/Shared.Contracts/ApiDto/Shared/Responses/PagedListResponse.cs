using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Shared.Contracts.ApiDto.Shared.Responses;

public record PagedListResponse<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount, 
    int TotalPages)
{
    public PagedListResponse(PagedList<T> pagedList) 
        : this(pagedList.Items, pagedList.Page, pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages)
    {
    }
}