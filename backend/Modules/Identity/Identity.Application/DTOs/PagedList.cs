namespace Qwestr.Modules.Identity.Application.DTOs;

public record PagedList<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
