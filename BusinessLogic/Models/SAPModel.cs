using BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class VendorCreateResponse
    {
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public List<SapReturn> SapMessages { get; set; }
    }


    public class VendorChangeResponse
    {
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public List<SapReturn> SapMessages { get; set; }
    }

    public class SapReturn
    {
        public string TYPE { get; set; }
        public string ID { get; set; }
        public string VENDORCODE { get; set; }
        public string MESSAGE { get; set; }
    }

    public class SAP_REQUEST_HEADER
    {
        public string RequestId { get; set; }
        public string ReqTransactionId { get; set; }
        public string ConsumerId { get; set; }
        public string ReqTimestamp { get; set; }
    }

    public class SAP_ZBAPI_CREATEVENDOR
    {
        public SAP_REQUEST_HEADER REQUEST_HEADER { get; set; }
        public SAP_VENDOR_ITEM VENDOR_ITEM { get; set; }
    }


    public class SAP_VENDOR_ITEM
    {
        // GENERAL
        public string LFA1_BRSCH { get; set; }
        public string RF02K_KTOKK { get; set; }

        public string LFA1_STCD1 { get; set; }
        public string LFA1_STCD2 { get; set; }
        public string LFA1_STCD3 { get; set; }
        public string LFA1_STCD4 { get; set; }

        // ADDRESS
        public string ADDR1_DATA_NAME1 { get; set; }
        public string ADDR1_DATA_NAME2 { get; set; }
        public string ADDR1_DATA_CITY1 { get; set; }
        public string ADDR1_DATA_CITY2 { get; set; }
        public string ADDR1_DATA_POST_CODE1 { get; set; }
        public string ADDR1_DATA_STREET { get; set; }
        public string ADDR1_DATA_STR_SUPPL3 { get; set; }
        public string ADDR1_DATA_LOCATION { get; set; }
        public string ADDR1_DATA_REGION { get; set; }

        // CONTACT
        public string SZAI_D0100_TEL_NUMBER { get; set; }
        public string SZAI_D0100_FAX_NUMBER { get; set; }
        public string SZAI_D0100_SMTP_ADDR { get; set; }

        // BANK
        public string LFBK_BANKS { get; set; }
        public string LFBK_BANKL { get; set; }
        public string LFBK_BANKN { get; set; }
        public string LFBK_BVTYP { get; set; }
        public string LFBK_BKREF { get; set; }
        public string LFBK_KOINH { get; set; }

        // COMPANY
        public string RF02K_BUKRS { get; set; }
        public string LFB1_ZUAWA { get; set; }
        public string LFB1_AKONT { get; set; }
        public string LFB1_ZWELS { get; set; }
        public string LFB1_ZTERM { get; set; }

        // PURCHASING
        public string LFM1_WAERS { get; set; }
        public string LFM1_KALSK { get; set; }

        // TAX
        public string LFA1_J_1KFTIND { get; set; }
        public string MSIC_CODE { get; set; }
    }

    public class SAP_ZBAPI_CHANGEVENDOR
    {
        public SAP_REQUEST_HEADER REQUEST_HEADER { get; set; }
        public SAP_VENDOR_CHANGE_ITEM VENDOR_ITEM { get; set; }
    }

    public class  SAP_VENDOR_CHANGE_ITEM
    {
        public string OBJ_TASK { get; set; }
        public string LIFNR { get; set; }
        public string BRSCH { get; set; }
        public string KTOKK { get; set; }

        public string STCD1 { get; set; }
        public string STCD2 { get; set; }
        public string STCD3 { get; set; }
        public string STCD4 { get; set; }

        public string NAME1 { get; set; }
        public string NAME2 { get; set; }

        // ADDRESS
        public string CITY1 { get; set; }
        public string CITY2 { get; set; }
        public string POST_CODE1 { get; set; }
        public string STREET { get; set; }
        public string STR_SUPPL3 { get; set; }
        public string LOCATION { get; set; }
        public string REGION { get; set; }

        public string SORT1 { get; set; }
        public string SORT2 { get; set; }

        // CONTACT
        public string TEL_NUMBER { get; set; }
        public string FAX_NUMBER { get; set; }
        public string SMTP_ADDR { get; set; }

        // BANK
        public string BANKS { get; set; }
        public string BANKL { get; set; }
        public string BANKN { get; set; }
        public string BVTYP { get; set; }
        public string BKREF { get; set; }
        public string KOINH { get; set; }

        // COMPANY
        public string BUKRS { get; set; }
        public string ZUAWA { get; set; }
        public string AKONT { get; set; }
        public string ZWELS { get; set; }
        public string ZTERM { get; set; }
        public string FDGRV { get; set; }
        public string REPRF { get; set; }
        public string WAERS { get; set; }

        // PURCHASING
        public string KALSK { get; set; }
        public string WEBRE { get; set; }
        public string LEBRE { get; set; }
        public string SPERM { get; set; }

        // Z FIELDS
        public string ZSETTLR { get; set; }
        public string ZKBDG { get; set; }
        public string ZSBDG { get; set; }
        public string ZACTVT { get; set; }
        public string ZSJLN { get; set; }

        public DateTime? ZDATST { get; set; }
        public DateTime? ZDATEN { get; set; }
        public string ZLOC { get; set; }

        public DateTime? ZDATDF { get; set; }
        public DateTime? ZDATRN { get; set; }
        public DateTime? ZDATBL { get; set; }
        public DateTime? ZDTBLN { get; set; }

        public string ZKSYKT { get; set; }
        public string ZRSBL { get; set; }

        // TAX
        public string J_1KFTIND { get; set; }
        public string MY_EXT_TIN_NUMBER { get; set; }
        public string FITYP { get; set; }
    }
}
