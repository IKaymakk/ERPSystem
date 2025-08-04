using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.DTOs.Category;

public class CategoryDto
{
    public int Id { get; set; } // Temel ID
    public string Code { get; set; } // ELK, ELK-PC gibi kategori kodu
    public string Name { get; set; } // Elektronik, Bilgisayar gibi isim
    public string Description { get; set; } 

    public int? ParentCategoryId { get; set; } // Üst kategori ID'si (null ise ana kategori)
    public string ParentCategoryName { get; set; } // Üst kategori ismi (UI'da göstermek için)

    public string FullPath { get; set; } // "Elektronik > Bilgisayar > Laptop"
    public int Level { get; set; } // 0=Ana, 1=Alt, 2=Alt-Alt... (tree depth)
    public bool HasChildren { get; set; } // Alt kategorisi var mı? (UI'da + - göstermek için)

    public int ProductCount { get; set; } // Bu kategorideki ürün sayısı

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsActive { get; set; }

    public List<CategoryDto> Children { get; set; } = new List<CategoryDto>();
}