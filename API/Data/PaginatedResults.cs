using System;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PaginatedResults<T>
{
    public PaginationMetaData MetaData { get; set; } = default!;
    public List<T> Items { get; set; } = [];
}

public class PaginationMetaData
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }
}

public class PaginationHelper
{
    public static async Task<PaginatedResults<T>> CreateAsync<T>(IQueryable<T> query,
    int pageNumber, int PageSize)
        {
                var count = await query.CountAsync();
                var items = await query.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToListAsync();

                return new PaginatedResults<T>
                {
                    MetaData = new PaginationMetaData
                    {
                        CurrentPage = pageNumber,
                        TotalPages = (int)Math.Ceiling(count / (double)PageSize),
                        PageSize = PageSize,
                        TotalCount = count
                    },
                    Items = items
                };
        }
}
