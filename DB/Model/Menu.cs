

namespace DB.EFModel
{
    public class Menu:BaseEntity
    {
        [Key]  // Marks as Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key
        public string? Name { get; set; }
        public string ModuleId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string? ParentId { get; set; }
        public string? Url { get; set; }
        public Boolean Status { get; set; }
        public int SeqId { get; set; }

        public Role? Role { get; set; }


    }
}
