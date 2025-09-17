namespace Foodly.Application.DTOs
{
    public record PagedQuery(int Page = 1, int PageSize = 8, string? Q = null, int? CategoryId = null);
}
