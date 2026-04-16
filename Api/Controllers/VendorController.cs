using Api.Controllers;
using Api.Models;
using AutoMapper;
using AutoMapper.Execution;
using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Entity.SAP;
using DB.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Procura.Models;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using YourNamespace.Services;

namespace Procura.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class VendorController : AuthorizedCSABaseAPIController
    {
        private readonly IContentService _contentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IVendorService _vendorService;
        private readonly ISAPServices _sapService;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient client = new HttpClient();


        public VendorController(IContentService contentService,
            ICurrentUserService currentUserService,
            IUserService userService, IVendorService vendorService, ISAPServices sapServices,
            ILogger<ContentController> logger, IConfiguration configuration)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _contentService = contentService;
            _vendorService = vendorService;
            _sapService = sapServices;
            _configuration = configuration;
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveStep(
        //int vendorId,
        //VendorRegistrationStep step,
        //[FromBody] object payload)
        //{
        //    await _vendorService.SaveStepAsync(vendorId, step, payload);

        //    return Ok("Step saved successfully.");
        //}
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveStep(int vendorId, VendorRegistrationStep step, StepSaveRequest request)
        {
            await _vendorService.SaveStepAsync(vendorId, step, request);
            return Ok("Step saved successfully.");
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterVendor(CreateVendorRequest request)
        {
            try
            {
                var vendor = await _vendorService.RegisterVendor(request);
                var result = await SearchSSM(request.RocNumber, "get-company-profile-document");
                if (!result.Success)
                    return BadRequest(result.Error);
                return Ok(new { Id = vendor.Id, CurrentStep = vendor.CurrentStep.ToString(), RocNumber = vendor.RocNumber, NextStep = vendor.NextStep.ToString(), ProfileResponse = result.Data });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        // Compact endpoint — Swagger shows a small JSON body (generic object)
        // The service will deserialize the payload based on 'step'.
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveStepRaw(int vendorId, VendorRegistrationStep step, [FromBody] JsonElement payload)
        {
            if (payload.ValueKind == JsonValueKind.Undefined || payload.ValueKind == JsonValueKind.Null)
                return BadRequest("Payload is required.");

            try
            {
                await _vendorService.SaveStepRawAsync(vendorId, step, payload);
                return Ok(new { message = "Step saved successfully." });
            }
            catch (JsonException jex)
            {
                return BadRequest($"Invalid JSON for step {step}: {jex.Message}");
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> SearchCompanyEntity()
        //{
        //    var url = "https://cidp.ssmsearch.com/get-search-entity";

        //    var apiKey = "d49f2d60-0af1-4451-810c-e309b22dfca8";
        //    var apiSecret = "bc3b0838-b285-4448-8fb6-d2fa1ab207ce";
        //    var clientRefNo = "REF001";

        //    var requestBody = new
        //    {
        //        regNo = "199701030038",
        //        entityType = "company"
        //        // OR use regNo = "20245656XXXX"
        //    };

        //    var json = System.Text.Json.JsonSerializer.Serialize(requestBody);

        //    var request = new HttpRequestMessage(HttpMethod.Post, url);

        //    request.Headers.Add("Accept", "application/json");
        //    request.Headers.Add("x-Gateway-APIKey", apiKey);
        //    request.Headers.Add("x-Gateway-APISecret", apiSecret);
        //    request.Headers.Add("x-Client-Ref-No", clientRefNo);

        //    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await client.SendAsync(request);

        //    var responseContent = await response.Content.ReadAsStringAsync();

        //    Console.WriteLine($"Status: {response.StatusCode}");
        //    Console.WriteLine(responseContent);
        //    return Ok();
        //}


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SearchCompanyEntity([FromBody] SsmSearchRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RegNo))
                return BadRequest("regNo is required.");

            var ssmSection = _configuration.GetSection("SSM");
            var apiKey = ssmSection["APIKey"];
            var apiSecret = ssmSection["SecretKey"];
            var useSandbox = bool.TryParse(ssmSection["UseSandbox"], out var parsed) && parsed;
            var baseUrl = useSandbox ? ssmSection["SandboxBaseUrl"] : ssmSection["ProductionBaseUrl"];

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret) || string.IsNullOrWhiteSpace(baseUrl))
                return StatusCode(StatusCodes.Status500InternalServerError, "SSM API configuration missing.");

            var url = $"{baseUrl.TrimEnd('/')}/get-status-entity";
            var clientRefNo = "REF001";

            var requestBody = new
            {
                regNo = request.RegNo,
                //entityType = string.IsNullOrWhiteSpace(request.EntityType) ? "company" : request.EntityType
            };
            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);

            var jsonres = JsonSerializer.Serialize(requestBody);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Headers.Add("Accept", "application/json");
            httpRequest.Headers.Add("x-Gateway-APIKey", apiKey);
            httpRequest.Headers.Add("x-Gateway-APISecret", apiSecret);
            httpRequest.Headers.Add("x-Client-Ref-No", clientRefNo);

            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(httpRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, $"Error calling SSM API: {ex.Message}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            await _vendorService.SaveSSMResponse(jsonres, responseContent);
            if (response.IsSuccessStatusCode)
            {
                // Return raw JSON from the SSM API
                return Content(responseContent, "application/json");
            }
            else
            {
                return StatusCode((int)response.StatusCode, responseContent);
            }
        }


        [NonAction]
        public async Task<(bool Success, string? Data, string? Error)> SearchSSM(string regNo, string searchType)
        {
            try
            {
                var ssmSection = _configuration.GetSection("SSM");

                var apiKey = ssmSection["APIKey"];
                var apiSecret = ssmSection["SecretKey"];
                var useSandbox = bool.TryParse(ssmSection["UseSandbox"], out var parsed) && parsed;
                var baseUrl = useSandbox ? ssmSection["SandboxBaseUrl"] : ssmSection["ProductionBaseUrl"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                    return (false, null, "SSM Base URL is not configured.");

                var url = $"{baseUrl.TrimEnd('/')}/{searchType}";
                var clientRefNo = Guid.NewGuid().ToString();

                var requestBody = new { regNo };
                var json = JsonSerializer.Serialize(requestBody);

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                httpRequest.Headers.Add("Accept", "application/json");
                httpRequest.Headers.Add("x-Gateway-APIKey", apiKey);
                httpRequest.Headers.Add("x-Gateway-APISecret", apiSecret);
                httpRequest.Headers.Add("x-Client-Ref-No", clientRefNo);

                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                await _vendorService.SaveSSMResponse(json, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return (false, null, $"SSM API Error ({(int)response.StatusCode}): {responseContent}");
                }

                return (true, responseContent, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Exception calling SSM API: {ex.Message}");
            }
        }





        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Profile(int vendorId, VendorProfileDto request)
        {
            var profile = await _vendorService.SaveProfileAsync(vendorId, request);
            return Ok(new { NextStep = profile.ToString() });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Members(int vendorId, VendorMemberDto request)
        {
            var member = await _vendorService.SaveMembersAsync(vendorId, request);
            return Ok(new { NextStep = member.ToString() });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Financial(int vendorId, VendorFinancialDto request)
        {
            var financial = await _vendorService.SaveFinancialAsync(vendorId, request);
            return Ok(new { NextStep = financial.ToString() });
        }

        /// <summary>
        /// Save categories during initial registration (vendor not yet SAP-approved).
        /// For post-approval category changes, use POST /api/CategoryCodeApproval/SubmitRequest instead.
        /// For eligibility validation, use GET /api/CategoryCodeApproval/ValidateEligibility.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ValidateCategoryChangeEligibility(int vendorId)
        {
            var result = await _vendorService.ValidateCategoryChangeAsync(vendorId);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Categories(int vendorId, VendorCategoryRequest request)
        {
            try
            {
                // Header-level pre-validation: freeze period and change-limit checks
                var eligibility = await _vendorService.ValidateCategoryChangeAsync(vendorId);
                if (!eligibility.IsEligible)
                {
                    return BadRequest(new { Errors = eligibility.Errors });
                }
                var categories = await _vendorService.SaveCategoriesAsync(vendorId, request);
                return Ok(new { NextStep = categories.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Experiences(int vendorId, List<VendorExperienceDto> request)
        {
            var experience = await _vendorService.SaveExperiencesAsync(vendorId, request);
            return Ok(new { NextStep = experience.ToString() });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Declaration(int vendorId, VendorDeclarationDto request)
        {
            var declaration = await _vendorService.SaveDeclarationAsync(vendorId, request);
            return Ok(new { NextStep = declaration.ToString() });
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Payment(int vendorId)
        {
            var paymentDetails = await _vendorService.GetPaymentDetailsAsync(vendorId);
            return Ok(paymentDetails);
        }




        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCompanyTypes()
        {
            var companyTypes = await _vendorService.GetCompanyTypes();
            return Ok(companyTypes);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCompanyEntitiesByTypeId(int TypeId)
        {
            var companyTypes = await _vendorService.GetCompanyEntitiesByTypeIdAsync(TypeId);
            return Ok(companyTypes);
        }



        [HttpGet]
        public async Task<IActionResult> GetVendorList()
        {
            var companyTypes = await _vendorService.GetVendorListAsync();
            return Ok(companyTypes);
        }

        [HttpGet]
        public async Task<IActionResult> GetVendorDashboard()
        {
            var result = await _vendorService.GetVendorDashboardAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetVendors([FromQuery] VendorSearchRequest request)
        {
            var result = await _vendorService.GetVendorListAsync(request);
            return Ok(result);
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> SAPSimulator()
        //{
        //    VendorCreateRequest_SAP_Dto request = new VendorCreateRequest_SAP_Dto();
        //    var result = _sapService.CreateVendorSAP(request);
        //    return Ok(result);
        //}


        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> RequestVendorCodeFromSAP(VendorCodeRequest request)
        //{
        //    var vendorrequest = new VendorCreateRequest_SAP_Dto
        //    {
        //        VendorId = request.VendorId,
        //        PaymentMethods = request.PaymentMethod,
        //        BankAccount = request.AccountNo,
        //        GR_INV = request.GR_InvoiceVerification

        //    };
        //    var result = _sapService.CreateVendorSAP(vendorrequest);
        //    return Ok(result);
        //}

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SAPSimulator()
        {
            VendorCreateRequest_SAP_Dto request = new VendorCreateRequest_SAP_Dto();
            // Await the Task result — previously returned a Task object which caused serialization
            var result = await _sapService.CreateVendorSAP(request);
            return Ok(result);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RequestVendorCodeFromSAP(VendorCodeRequest request)
        {
            var vendorrequest = new VendorCreateRequest_SAP_Dto
            {
                VendorId = request.VendorId,
                PaymentMethods = request.PaymentMethod,
                BankAccount = request.AccountNo,
                GR_INV = request.GR_InvoiceVerification

            };
            // Await the Task result — previously returned the Task object directly
            var result = await _sapService.CreateVendorSAP(vendorrequest);
            return Ok(result);
        }



        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RequestVendorDetailsUpdateFromSAP(VendorDetailsChangeRequest request)
        {
            var vendorrequest = new VendorChangeRequest_SAP_Dto
            {
                VendorId = request.VendorId,
            };
            // Await the Task result — previously returned the Task object directly
            var result = await _sapService.ChangeVendorSAP(vendorrequest);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetIndustryTypeList()
        {
            var result = await _vendorService.BindIndustryTypeListAsync();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetVendorDetilsById(int vendorId)
        {
            var result = await _vendorService.GetVendorFullDetailsAsync(vendorId);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveQuestionAnswers(int vendorId, List<QuestionAnswerDto> answers)
        {
            await _vendorService.SaveQuestionAnswers(vendorId, answers);
            return Ok(new { message = "Question answers saved successfully." });
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetQuestionAnswers(int questionnaireId, int vendorId)
        {
            var answers = await _vendorService.GetQuestionAnswersByQuestionnaireId(questionnaireId, vendorId);
            return Ok(answers);
        }

    }
}