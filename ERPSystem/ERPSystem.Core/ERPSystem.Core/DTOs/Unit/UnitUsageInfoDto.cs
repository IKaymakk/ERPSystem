using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.DTOs.Unit
{
    public class UnitUsageInfoDto
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitSymbol { get; set; }
        public int ProductCount { get; set; }
        public bool CanBeDeleted => ProductCount == 0;
        public string UsageDetails { get; set; }

        // Factory method
        public static UnitUsageInfoDto Create(int unitId, string unitName, string unitSymbol, int productCount)
        {
            return new UnitUsageInfoDto
            {
                UnitId = unitId,
                UnitName = unitName,
                UnitSymbol = unitSymbol,
                ProductCount = productCount,
                UsageDetails = productCount > 0
                    ? $"Bu unit {productCount} adet üründe kullanılmaktadır"
                    : "Bu unit herhangi bir üründe kullanılmamaktadır"
            };
        }
    }

}
