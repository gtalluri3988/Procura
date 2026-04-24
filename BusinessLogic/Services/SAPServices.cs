using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using DB.Entity.SAP;
using DB.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace BusinessLogic.Services
{
    public class SAPServices : ISAPServices
    {

        //var service = new SAPServices();
        //var response = service.CreateVendorSAP(requestDto);
        private readonly IZBAPI_CREATEVENDORClient _sapClient;
        private readonly IVendorRepository _vendorRepository;
        private readonly IVendorService _vendorService;
        private readonly ILogger<SAPServices> _logger;

        public SAPServices(
            IZBAPI_CREATEVENDORClient sapClient,
            IVendorRepository vendorRepository,
            IVendorService vendorService,
            ILogger<SAPServices> logger)
        {
            _sapClient = sapClient;
            _vendorRepository = vendorRepository;
            _vendorService = vendorService;
            _logger = logger;
        }

        public async Task<VendorCreateResponse> CreateVendorSAP(VendorCreateRequest_SAP_Dto request)
        {
            var v = await _vendorRepository.GetSAPVendorByVendorIdAsync(request.VendorId);
            if (v != null)
            {
                request = new VendorCreateRequest_SAP_Dto()
                {
                    RequestId = Guid.NewGuid().ToString(),
                    ReqTransactionId = Guid.NewGuid().ToString(),
                    ConsumerId = Guid.NewGuid().ToString(),
                    ReqTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Industry = v.IndustryType?.Name,
                    AccGroup = "EDOM",
                    GSTNo = v.VendorFinancial?.Tax?.SstNo,
                    SSTSalesNo = v.VendorFinancial?.Tax?.SstNo,
                    SSTServiceNo = v.VendorFinancial?.Tax?.SstNo,
                    ROC = v.RocNumber,
                    // ADDRESS
                    Name = v.CompanyName,
                    Name2 = v.CompanyName,
                    City = v.City,
                    District = "PUCHONG",
                    PostalCode = v.Postcode,
                    Street = "",
                    Street4 = "",
                    Street5 = "",
                    Region = "",
                    // CONTACT
                    Telephone = v.OfficePhoneNo,
                    Fax = v.FaxNo,
                    Email = v.Email,
                    //can have multiple need to check with team
                    // BANK
                    BankCountry = v.VendorFinancial?.Bank?.AccountNumber,
                    BankKey = v.VendorFinancial?.Bank?.AccountNumber,
                    BankAccount = v.VendorFinancial?.Bank?.AccountNumber,
                    BankType = v.VendorFinancial?.Bank?.BankName,
                    BankRef = v.VendorFinancial?.Bank?.AccountNumber,
                    AccountHolder = v.VendorFinancial?.Bank?.AccountHolderName,
                    // COMPANY
                    CompanyCode = "",
                    SortKey = "",
                    ReconAccount = "",
                    PaymentMethods = "",
                    PaymentTerms = "",
                    // PURCHASING
                    Currency = "",
                    SchemaVendor = "",
                    // TAX
                    TINNo = v.VendorFinancial?.Tax?.TinNo,
                    MSICCode = v.VendorFinancial?.Tax?.MsicCode
                };
            }
            var sapRequest = BuildSapVendor(request);
            var requestJson = JsonSerializer.Serialize(
            sapRequest,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
            var sapResponse = _sapClient.Execute(sapRequest);
            string responseJson = JsonSerializer.Serialize(sapResponse, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await _vendorRepository.SaveSAPRequestResponseAsync(request.VendorId, requestJson, responseJson);

            // Persist approval + trigger email when SAP returns a vendor code and no error rows
            try
            {
                var returnRows = sapResponse?.RETURN ?? Array.Empty<BAPIRET2>();
                var hasError = returnRows.Any(r => string.Equals(r?.TYPE, "E", StringComparison.OrdinalIgnoreCase));
                var vendorCode = returnRows
                    .Select(r => r?.VENDORCODE)
                    .FirstOrDefault(code => !string.IsNullOrWhiteSpace(code));

                if (!hasError && !string.IsNullOrWhiteSpace(vendorCode))
                {
                    var marked = await _vendorRepository.MarkVendorApprovedAsync(request.VendorId, vendorCode!, DateTime.UtcNow);
                    if (marked)
                    {
                        await _vendorService.SendApprovalConfirmationAsync(request.VendorId);
                    }
                }
            }
            catch (Exception ex)
            {
                // Never fail the SAP response path because of post-processing (persist or email)
                _logger.LogError(ex, "SAP post-processing (mark approved / send email) failed for vendor {VendorId}", request.VendorId);
            }

            return new VendorCreateResponse
            {
                RespCode = "0000",
                RespDesc = "Success",
                SapMessages = sapResponse.RETURN.Select(x => new SapReturn
                {
                    TYPE = x.TYPE,
                    ID = x.ID,
                    VENDORCODE = x.VENDORCODE,
                    MESSAGE = x.MESSAGE
                }).ToList()
            };
        }

        private SAP_ZBAPI_CREATEVENDOR BuildSapVendor(VendorCreateRequest_SAP_Dto v)
        {
            return new SAP_ZBAPI_CREATEVENDOR
            {
                REQUEST_HEADER = new SAP_REQUEST_HEADER
                {
                    RequestId = v.RequestId ?? "",
                    ReqTransactionId = v.ReqTransactionId ?? "",
                    ConsumerId = v.ConsumerId ?? "",
                    ReqTimestamp = v.ReqTimestamp ?? ""
                },

                VENDOR_ITEM = new SAP_VENDOR_ITEM
                {
                    // GENERAL
                    LFA1_BRSCH = v.Industry ?? "",
                    RF02K_KTOKK = v.AccGroup ?? "",

                    LFA1_STCD1 = v.GSTNo ?? "",
                    LFA1_STCD2 = v.SSTSalesNo ?? "",
                    LFA1_STCD3 = v.SSTServiceNo ?? "",
                    LFA1_STCD4 = v.ROC,

                    // ADDRESS
                    ADDR1_DATA_NAME1 = v.Name,
                    ADDR1_DATA_NAME2 = v.Name2 ?? "",
                    ADDR1_DATA_CITY1 = v.City,
                    ADDR1_DATA_CITY2 = v.District,
                    ADDR1_DATA_POST_CODE1 = v.PostalCode ?? "",
                    ADDR1_DATA_STREET = v.Street ?? "",
                    ADDR1_DATA_STR_SUPPL3 = v.Street4 ?? "",
                    ADDR1_DATA_LOCATION = v.Street5 ?? "",
                    ADDR1_DATA_REGION = v.Region ?? "",

                    // CONTACT
                    SZAI_D0100_TEL_NUMBER = v.Telephone,
                    SZAI_D0100_FAX_NUMBER = v.Fax,
                    SZAI_D0100_SMTP_ADDR = v.Email ?? "",

                    // BANK
                    LFBK_BANKS = v.BankCountry ?? "",
                    LFBK_BANKL = v.BankKey ?? "",
                    LFBK_BANKN = v.BankAccount ?? "",
                    LFBK_BVTYP = v.BankType ?? "",
                    LFBK_BKREF = v.BankRef ?? "",
                    LFBK_KOINH = v.AccountHolder ?? "",

                    // COMPANY
                    RF02K_BUKRS = v.CompanyCode ?? "",
                    LFB1_ZUAWA = v.SortKey ?? "",
                    LFB1_AKONT = v.ReconAccount ?? "",
                    LFB1_ZWELS = v.PaymentMethods ?? "",
                    LFB1_ZTERM = v.PaymentTerms,

                    // PURCHASING
                    LFM1_WAERS = v.Currency,
                    LFM1_KALSK = v.SchemaVendor,

                    // TAX
                    LFA1_J_1KFTIND = v.TINNo ?? "",
                    MSIC_CODE = v.MSICCode ?? ""
                }
            };
        }



        public async Task<VendorChangeResponse> ChangeVendorSAP(VendorChangeRequest_SAP_Dto request)
        {
            var v = await _vendorRepository.GetSAPVendorByVendorIdAsync(request.VendorId);
            if (v != null)
            {
                request = new VendorChangeRequest_SAP_Dto()
                {
                    RequestId = Guid.NewGuid().ToString(),
                    ReqTransactionId = Guid.NewGuid().ToString(),
                    ConsumerId = Guid.NewGuid().ToString(),
                    ReqTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Industry = v.IndustryType?.Name,
                    AccGroup = "EDOM",
                   // GSTNo = v.VendorFinancial?.Tax?.SstNo,
                   // SSTSalesNo = v.VendorFinancial?.Tax?.SstNo,
                   // SSTServiceNo = v.VendorFinancial?.Tax?.SstNo,
                    ROC = v.RocNumber,
                    // ADDRESS
                    Name1 = v.CompanyName,
                    Name2 = v.CompanyName,
                    City = v.City,
                    District = "PUCHONG",
                    PostalCode = v.Postcode,
                    Street = "",
                    Street4 = "",
                    Street5 = "",
                    Region = "",
                    // CONTACT
                    Telephone = v.OfficePhoneNo,
                    Fax = v.FaxNo,
                    Email = v.Email,
                    //can have multiple need to check with team
                    // BANK
                    BankCountry = v.VendorFinancial?.Bank?.AccountNumber,
                    BankKey = v.VendorFinancial?.Bank?.AccountNumber,
                    BankAccount = v.VendorFinancial?.Bank?.AccountNumber,
                    BankType = v.VendorFinancial?.Bank?.BankName,
                    //BankRef = v.VendorFinancial?.Bank?.AccountNumber,
                    AccountHolder = v.VendorFinancial?.Bank?.AccountHolderName,
                    // COMPANY
                    CompanyCode = "",
                    SortKey = "",
                    ReconAccount = "",
                    PaymentMethods = "",
                    PaymentTerms = "",
                    // PURCHASING
                    Currency = "",
                    SchemaVendor = "",
                    // TAX
                    //TINNo = v.VendorFinancial?.Tax?.TinNo,
                    //MSICCode = v.VendorFinancial?.Tax?.MsicCode
                };
            }
            var sapRequest = BuildSapChangeVendor(request);
            var requestJson = JsonSerializer.Serialize(
            sapRequest,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
            var sapResponse = _sapClient.Execute(sapRequest);
            string responseJson = JsonSerializer.Serialize(sapResponse, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await _vendorRepository.SaveSAPRequestResponseAsync(request.VendorId, requestJson, responseJson);
            return new VendorChangeResponse
            {
                RespCode = "0000",
                RespDesc = "Success",
                SapMessages = sapResponse.RETURN.Select(x => new SapReturn
                {
                    TYPE = x.TYPE,
                    ID = x.ID,
                    VENDORCODE = x.VENDORCODE,
                    MESSAGE = x.MESSAGE
                }).ToList()
            };
        }

        private SAP_ZBAPI_CHANGEVENDOR BuildSapChangeVendor(VendorChangeRequest_SAP_Dto v)
        {
            return new SAP_ZBAPI_CHANGEVENDOR
            {
                REQUEST_HEADER = new SAP_REQUEST_HEADER
                {
                    RequestId = v.RequestId ?? "",
                    ReqTransactionId = v.ReqTransactionId ?? "",
                    ConsumerId = v.ConsumerId ?? "",
                    ReqTimestamp = v.ReqTimestamp ?? ""
                },

                VENDOR_ITEM = new SAP_VENDOR_CHANGE_ITEM
                {
                    //public string OBJ_TASK { get; set; }
                    //public string LIFNR { get; set; }
                    //public string BRSCH { get; set; }
                    //public string KTOKK { get; set; }

                    //public string STCD1 { get; set; }
                    //public string STCD2 { get; set; }
                    //public string STCD3 { get; set; }
                    //public string STCD4 { get; set; }

                    //public string NAME1 { get; set; }
                    //public string NAME2 { get; set; }

                    //// ADDRESS
                    //public string CITY1 { get; set; }
                    //public string CITY2 { get; set; }
                    //public string POST_CODE1 { get; set; }
                    //public string STREET { get; set; }
                    //public string STR_SUPPL3 { get; set; }
                    //public string LOCATION { get; set; }
                    //public string REGION { get; set; }

                    //public string SORT1 { get; set; }
                    //public string SORT2 { get; set; }

                    //// CONTACT
                    //public string TEL_NUMBER { get; set; }
                    //public string FAX_NUMBER { get; set; }
                    //public string SMTP_ADDR { get; set; }

                    //// BANK
                    //public string BANKS { get; set; }
                    //public string BANKL { get; set; }
                    //public string BANKN { get; set; }
                    //public string BVTYP { get; set; }
                    //public string BKREF { get; set; }
                    //public string KOINH { get; set; }

                    //// COMPANY
                    //public string BUKRS { get; set; }
                    //public string ZUAWA { get; set; }
                    //public string AKONT { get; set; }
                    //public string ZWELS { get; set; }
                    //public string ZTERM { get; set; }
                    //public string FDGRV { get; set; }
                    //public string REPRF { get; set; }
                    //public string WAERS { get; set; }

                    //// PURCHASING
                    //public string KALSK { get; set; }
                    //public string WEBRE { get; set; }
                    //public string LEBRE { get; set; }
                    //public string SPERM { get; set; }

                    //// Z FIELDS
                    //public string ZSETTLR { get; set; }
                    //public string ZKBDG { get; set; }
                    //public string ZSBDG { get; set; }
                    //public string ZACTVT { get; set; }
                    //public string ZSJLN { get; set; }

                    //public DateTime? ZDATST { get; set; }
                    //public DateTime? ZDATEN { get; set; }
                    //public string ZLOC { get; set; }

                    //public DateTime? ZDATDF { get; set; }
                    //public DateTime? ZDATRN { get; set; }
                    //public DateTime? ZDATBL { get; set; }
                    //public DateTime? ZDTBLN { get; set; }

                    //public string ZKSYKT { get; set; }
                    //public string ZRSBL { get; set; }

                    //// TAX
                    //public string J_1KFTIND { get; set; }
                    //public string MY_EXT_TIN_NUMBER { get; set; }
                    //public string FITYP { get; set; }
                    // GENERAL
                    OBJ_TASK = v.OBJ_TASK ?? "",
                    LIFNR=v.ZSJLN??""
                        
                   
                }
            };
        }



    }
}