

namespace DB.EFModel
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }

        public int CodeMasterId { get; set; }

        public string CategoryName { get; set; }

        public bool IsActive { get; set; }

        public CodeMaster CodeMaster { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }
    }
}
