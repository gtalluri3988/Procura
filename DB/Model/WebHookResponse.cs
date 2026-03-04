namespace DB.EFModel
{
    public class WebHookResponse : BaseEntity
    {
        [Key]  // Marks as Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key
        public string? Response { get; set; }
        public DateTime ResponseDateTime { get; set; }
    }



}

