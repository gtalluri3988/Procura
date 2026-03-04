
namespace DB.EFModel
{
    public class ContentPicture : BaseEntity
    {
        [Key]  // Marks as Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key
        public string? PhotoPath { get; set; }
        public string? ImageGuid { get; set; } // 
        public string? Preview { get; set; }
        public string? Name { get; set; }
        public int ContentManagementId { get; set; }
        
    }
}
