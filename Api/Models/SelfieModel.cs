namespace Api.Models
{
    
    public class SelfieUploadModel
    {
        public int VehicleId { get; set; }
        public string? ImageBase64 { get; set; }

        public int ResidentId { get; set; }
    }

}
