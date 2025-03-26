using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace MeSoftCase.WebApi.Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> ProjectToPaginatedListAsync<TSource, TDestination>(this IQueryable<TSource> queryable, IConfigurationProvider configuration, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        => PaginatedList<TDestination>.CreateAsync(queryable, configuration, pageNumber, pageSize, cancellationToken);

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration, CancellationToken cancellationToken = default)
        => queryable.ProjectTo<TDestination>(configuration).ToListAsync(cancellationToken);
}

public class PaginatedList<TDestination>(List<TDestination> items, int count, int pageNumber, int pageSize)
{
    public List<TDestination> Items { get; } = items;
    public int PageNumber { get; } = pageNumber;
    public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);
    public int TotalCount { get; } = count;
    public int Size { get; } = pageSize;

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<TDestination>> CreateAsync<TSource>(IQueryable<TSource> source, IConfigurationProvider configuration, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectToListAsync<TDestination>(configuration, cancellationToken);

        return new PaginatedList<TDestination>(items, count, pageNumber, pageSize);
    }
}

