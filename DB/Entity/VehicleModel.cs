using DB.EFModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace DB.Entity
{
    public class VehicleModelDTO
    {
        public int? Id { get; set; }  // Primary Key
        public string? VehicleNo { get; set; }  // Primary Key
        public int ResidentId { get; set; }  // Primary Key
        public int VehicleTypeId { get; set; }
        public string? FileName { get; set; }  // Primary Key
        public string? ImagePath { get; set; }
    }
}
