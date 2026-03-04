

namespace DB.EFModel
{
    public class EventDetails:BaseEntity
    {
        [Key]  // Marks as Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key
        public string? EventDescription { get; set; }  
        public DateTime EventStart { get; set; }
        public string? EventRefNo { get; set; }
        public DateTime EventEnd { get; set; }  
        public int? CommunityId { get; set; }
        public int? ResidentId { get; set; }
        public int? QrScanLimit { get; set; }
       
    }
}
