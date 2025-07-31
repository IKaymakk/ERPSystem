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
        public static PagedResultDto<TDestination> ToPagedResult<TSource, TDestination>(this PagedResultDto<TSource> source, IMapper mapper)
        {
            var mappedItems = mapper.Map<IEnumerable<TDestination>>(source.Items);

            return new PagedResultDto<TDestination>
            {
                Items = mappedItems,
                TotalCount = source.TotalCount,
                PageNumber = source.PageNumber,
                PageSize = source.PageSize
            };
        }
    }
}
