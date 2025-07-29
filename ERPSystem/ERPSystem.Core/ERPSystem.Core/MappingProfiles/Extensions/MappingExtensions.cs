using AutoMapper;
using ERPSystem.Core.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace ERPSystem.Core.MappingProfiles.Extensions
{
    public static class MappingExtensions
    {
        public static async Task<PagedResultDto<TDestination>> ToPagedResultAsync<TSource, TDestination>(
            this IQueryable<TSource> source,
            PagedRequestDto request,
            IMapper mapper)
        {
            var totalCount = await source.CountAsync();

            var items = await source
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var mappedItems = mapper.Map<IEnumerable<TDestination>>(items);

            return new PagedResultDto<TDestination>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public static PagedResultDto<TDestination> ToPagedResult<TSource, TDestination>(
            this IEnumerable<TSource> source,
            PagedRequestDto request,
            IMapper mapper,
            int totalCount)
        {
            // Skip/Take'i kaldır - Repository zaten yapmış
            var mappedItems = mapper.Map<IEnumerable<TDestination>>(source);
            return new PagedResultDto<TDestination>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
