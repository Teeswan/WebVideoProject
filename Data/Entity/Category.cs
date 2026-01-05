using System;
using System.Collections.Generic;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class Category
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid(); // Auto GUID
        public string Name { get; set; } = null!;
        public Guid? ParentCategoryId { get; set; } // Nullable

        // Navigation
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Video> Videos { get; set; } = new List<Video>();
    }
}
