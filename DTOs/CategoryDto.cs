namespace Youtube_Entertainment_Project.DTOs
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public Guid? ParentCategoryId { get; set; }
    }
}
