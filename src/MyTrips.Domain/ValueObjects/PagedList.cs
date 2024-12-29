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
}