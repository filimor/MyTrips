﻿namespace MyTrips.Domain.ValueObjects;

public class ClientParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;
    public int PageIndex { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;

        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}