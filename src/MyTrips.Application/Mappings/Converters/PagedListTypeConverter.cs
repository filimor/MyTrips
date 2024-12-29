using AutoMapper;
using MyTrips.Domain.ValueObjects;

namespace MyTrips.Application.Mappings.Converters;

public class PagedListTypeConverter<TSource, TDestination>
    : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
{
    public PagedList<TDestination> Convert(PagedList<TSource> source,
        PagedList<TDestination> destination, ResolutionContext context)
    {
        var mappedItems = source.Select(item => context.Mapper.Map<TDestination>(item)).ToList();
        return new PagedList<TDestination>(mappedItems, source.CurrentPage, source.TotalPages, source.PageSize);
    }
}