namespace Luka.CQRS.Queries;

public class PagedQueryBase : IPagedQuery
{
    public int Page { get; set; } = 1;
    public int Results { get; set; } = 10;
    public string? OrderBy { get; set; }
    public string? SortOrder { get; set; }
}
