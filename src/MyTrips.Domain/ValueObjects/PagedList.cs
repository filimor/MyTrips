namespace MyTrips.Domain.ValueObjects;

public class PagedList<T> : List<T>
{
    public PagedList(List<T> items, int pageIndex, int pageCount, int pageSize)
    {
        CurrentPage = pageIndex;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(pageCount / (double)pageSize);

        AddRange(items);
    }

    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; private set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, pageIndex, count, pageSize);
    }
}