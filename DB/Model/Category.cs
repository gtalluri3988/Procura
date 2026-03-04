

namespace DB.EFModel
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }  // Primary Key
        public string Name { get; set; }

        // Navigation Property (One Category has many Products)
        public List<Product> Products { get; set; }
    }
}
