using Microsoft.EntityFrameworkCore;

namespace book_backend.utils;

public static class PaginationUtil
{
    /// <summary>
    /// 封装通用的分页查询方法。
    /// this IQueryable query代表扩展的IQueryable查询，只能在静态类、静态方法上这么写
    /// </summary>
    /// <param name="query">未执行的IQueryable查询</param>
    /// <param name="pageIndex">当前页码（从1开始）</param>
    /// <param name="pageSize">每页大小</param>
    /// <typeparam name="T">数据泛型</typeparam>
    /// <returns>包含分页数据的对象</returns>
    public static async Task<Pagination<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> query,
        int pageIndex,
        int pageSize)
    {
        if (pageIndex < 1)
        {
            pageIndex = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Pagination<T>()
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Items = items
        };
    }
}