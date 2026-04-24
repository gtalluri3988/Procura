namespace DB.EFModel
{
    public class VendorReminderLog : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int VendorId { get; set; }

        [Required]
        public int ThresholdDays { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public DateTime SentAt { get; set; }

        public Vendor? Vendor { get; set; }
    }
}
