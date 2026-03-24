using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderJobScope:BaseEntity
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }

        public string IOWBS { get; set; }
        public string MaterialGroup { get; set; }
        public string MaterialGroupDescription { get; set; }
        public string ServiceCode { get; set; }
        public string ShortText { get; set; }

        public decimal Budget { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal SubTotal { get; set; }
        [JsonIgnore]
        public TenderApplication? TenderApplication { get; set; }
    }
}
