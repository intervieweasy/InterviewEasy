using Microsoft.EntityFrameworkCore;

namespace InterviewEasy.BuildingBlocks.Common.Pagination;

public sealed class PagedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedList(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
    }

    public static PagedList<T> Empty(PaginationParams p)
        => new(Array.Empty<T>(), 0, p.PageNumber, p.PageSize);

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> source,
        PaginationParams p,
        CancellationToken ct = default)
    {
        var total = await source.CountAsync(ct);
        var items = await source
            .Skip(p.Skip)
            .Take(p.PageSize)
            .ToListAsync(ct);

        return new PagedList<T>(items, total, p.PageNumber, p.PageSize);
    }
}