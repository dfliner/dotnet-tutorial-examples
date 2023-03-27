using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Utilities;

public class PaginatedList<T> : List<T>, IEnumerable<T>
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }

    public PaginatedList(IEnumerable<T> items, int itemCount, int pageIndex, int pageSize)
        : base(items)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(itemCount / (double)pageSize);
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}
