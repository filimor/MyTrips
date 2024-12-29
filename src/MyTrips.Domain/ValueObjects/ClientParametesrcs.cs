namespace MyTrips.Domain.ValueObjects;

public class ClientParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    /// <summary>
    /// The index of the page to get
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// The size of the page to get
    /// </summary>
    public int PageSize
    {
        get => _pageSize;

        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}