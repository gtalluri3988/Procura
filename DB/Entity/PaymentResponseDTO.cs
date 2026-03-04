using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class PaymentResponseDTO
    {
        
            public int Ret { get; set; }
            public string Msg { get; set; }
        public long PaymentId { get; set; }
        public long MerchantId { get; set; }
            public string OrderId { get; set; }
            public string TxId { get; set; }

            public string TxType { get; set; }
            public string TxStatus { get; set; }

            public DateTime TxDt { get; set; }

            public string TxAmount { get; set; }
            public string TxCurrency { get; set; }
            public string TxChannel { get; set; }

            public string RespCode { get; set; }
            public string SchemeId { get; set; }

            public int? PaymentTypeId { get; set; }


    }
}
