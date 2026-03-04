using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity.SAP
{
    public class VendorCreateRequest_SAP_Dto
    {
        public int VendorId { get; set; }
        public string PaymentMethod { get; set; }
        // HEADER
        public string? RequestId { get; set; }
        public string? ReqTransactionId { get; set; }
        public string? ConsumerId { get; set; }
        public string? ReqTimestamp { get; set; }

        // GENERAL
        public string? Industry { get; set; }        // LFA1-BRSCH
        public string? AccGroup { get; set; }        // KTOKK
        public string? GSTNo { get; set; }           // STCD1
        public string? SSTSalesNo { get; set; }      // STCD2
        public string? SSTServiceNo { get; set; }    // STCD3
        public string ROC { get; set; }             // STCD4

        public string Name { get; set; }            // NAME1
        public string? Name2 { get; set; }           // NAME2

        // ADDRESS
        public string City { get; set; }
        public string District { get; set; }
        public string? PostalCode { get; set; }
        public string? Street { get; set; }
        public string? Street4 { get; set; }
        public string? Street5 { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }

        public string SearchTerm1 { get; set; }
        public string SearchTerm2 { get; set; }

        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string? Email { get; set; }

        // BANK
        public string? BankCountry { get; set; }
        public string? BankKey { get; set; }
        public string? BankAccount { get; set; }
        public string? BankType { get; set; }
        public string? BankRef { get; set; }
        public string? AccountHolder { get; set; }

        // COMPANY CODE
        public string? CompanyCode { get; set; }
        public string? SortKey { get; set; }
        public string ReconAccount { get; set; }

        public string? PaymentMethods { get; set; }
        public string PaymentTerms { get; set; }
        public string? CashGroup { get; set; }
        public string? CheckInvoice { get; set; }
        public string Currency { get; set; }
        public string SchemaVendor { get; set; }

        public string GR_INV { get; set; }
        public string SRV_INV { get; set; }

        // Z Fields
        public string? ZSETTLR { get; set; }
        public string? ZKBDG { get; set; }
        public string? ZSBDG { get; set; }
        public string? ZACTVT { get; set; }
        public string? ZSJLN { get; set; }

        public DateTime? ZDATST { get; set; }
        public DateTime? ZDATEN { get; set; }
        public string? ZLOC { get; set; }

        public DateTime? ZDATDF { get; set; }
        public DateTime? ZDATRN { get; set; }
        public DateTime? ZDATBL { get; set; }
        public DateTime? ZDTBLN { get; set; }

        public string? ZKSYKT { get; set; }
        public string? ZRSBL { get; set; }

        // TAX
        public string? MSICCode { get; set; }
        public string? TINNo { get; set; }
        public string? TaxType { get; set; }
    }
}
