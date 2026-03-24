using Api.Helpers;
using Api.Models;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using BusinessLogic.Services;
using DB.EFModel;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using YourNamespace.Services;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentController : AuthorizedCSABaseAPIController
    {
        private readonly IAmpersandClient _ampClient;
        private readonly AmpersandOptions _opts;
        private readonly IOrderStore _orderStore;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null };

        private readonly ICurrentUserService _currentUserService;
        
        public readonly IPaymentService _paymentService;
        public readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        public PaymentController(
            ICurrentUserService currentUserService, IAmpersandClient ampClient, IOptions<AmpersandOptions> opts, IOrderStore orderStore,
          
            IUserService userService,
            IPaymentService paymentService,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            ILogger<PaymentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
           
            _paymentService = paymentService;
            _config= config;
            _httpClientFactory = httpClientFactory;
            _ampClient = ampClient;
            _opts = opts.Value;
            _orderStore = orderStore;
        }
        //[HttpGet]
        //public async Task<IActionResult> GetAllPayments()
        //{
        //    return Ok(await _paymentService.GetAllPaymentsAsync());
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetPaymentById(int payId)
        //{
        //    return Ok(await _paymentService.GetPaymentByIdAsync(payId));
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreatePayment(PaymentDTO payment)
        //{
        //    var createdPayment = await _paymentService.CreatePaymentAsync(payment);
        //    return CreatedAtAction(nameof(GetPaymentById), new { id = createdPayment.Id }, createdPayment);
        //}
        //[HttpPost]
        //public async Task<IActionResult> UpdatePayment(int id, PaymentDTO dto)
        //{
        //    await _paymentService.UpdatePaymentAsync(id, dto);
        //    return NoContent();
        //}

        //[HttpGet]
        //public async Task<IActionResult> TotalMaintanenceAmountByCommunity(int communityId, int paymentTypeId)
        //{
        //    if (IsResidentAdmin())
        //    {
        //        var PaymentTotal = await _paymentService.GetMaintanenceFeeTotalByCommunity(communityId, paymentTypeId, includeCommunityId: true);
        //        var response = new { PaymentTypeId = paymentTypeId, TotalAmount = PaymentTotal.ToString("N2") };
        //        return Ok(response);
        //    }
        //    if (IsCSAAdmin())
        //    {
        //        var PaymentTotal = await _paymentService.GetMaintanenceFeeTotalByCommunity(communityId, paymentTypeId);
        //        var response = new { PaymentTypeId = paymentTypeId, TotalAmount = PaymentTotal.ToString("N2") };
        //        return Ok(response);
        //    }
        //    return NotFound();
        //}

        [HttpGet]
        public async Task<IActionResult> PaymentDetails()
        {
            var gatewayParameter = new GatewayParameter
            {
                apiKey = "a0cf8bbcd3f886be",
                TID = "VACCINEAPP",
                orderNo = $"INV{new Random().Next(1000)}_STATIC",
                orderDescription = "Vaccination Price",
                currency = "MYR",
                amount = "150.00",
                method = "CC",
                apiOperation = "SALE",
                cardType = "MASTERCARD",
                email = "gowtham.netapp@gmail.com"
            };

            string raw = gatewayParameter.apiKey +
                         gatewayParameter.TID +
                         gatewayParameter.orderNo +
                         gatewayParameter.orderDescription +
                         gatewayParameter.currency +
                         gatewayParameter.amount +
                         gatewayParameter.method +
                         gatewayParameter.apiOperation +
                         gatewayParameter.cardType +
                         gatewayParameter.email;

            gatewayParameter.checksum = GenerateSHA256String(raw);

            return Ok(gatewayParameter);
        }

        private static string GenerateSHA256String(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }


        //public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequest payment)
        //{
        //    var jsonPayload = JsonConvert.SerializeObject(payment);
        //    var signature = ComputeSha512Hash(jsonPayload , _config["PaymentGateway:APIKey"]);

        //    var client = _httpClientFactory.CreateClient();
        //    var request = new HttpRequestMessage(HttpMethod.Post, _config["PaymentGateway:Url"]);
        //    request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        //    request.Headers.Add("signature", signature);

        //    var response = await client.SendAsync(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var result = await response.Content.ReadAsStringAsync();
        //        var paymentResp = JsonConvert.DeserializeObject<PaymentResponse>(result);
        //        return Ok(paymentResp);
        //    }
        //    else
        //    {
        //        var error = await response.Content.ReadAsStringAsync();
        //        return StatusCode((int)response.StatusCode, error);
        //    }
        //}

        //private string? ComputeSha512Hash(string RequestBody, string SecretKey)
        //{
        //    var signature = Api.Helpers.SignatureHelper.GenerateSignature(RequestBody, SecretKey);
        //    return signature;
        //}
        //public async Task<IActionResult> PaymentCallback()
        //{
        //    string jsonData;
        //    using (var reader = new StreamReader(Request.Body))
        //    {
        //        jsonData = await reader.ReadToEndAsync();
        //    }

        //    var payload = JObject.Parse(jsonData);
        //    System.IO.File.AppendAllText("Logs/webhook_log.txt", $"Received at {DateTime.Now}: {jsonData}\n");

        //    var model = new Api.Models.CrmEntity
        //    {
        //        TxId = payload["txId"]?.ToString(),
        //        OrderId = payload["orderId"]?.ToString(),
        //        TxChannel = payload["txChannel"]?.ToString(),
        //        TxStatus = payload["txStatus"]?.ToString(),
        //        TxDt = payload["txDt"]?.ToString(),
        //        // Add other required fields
        //    };

        //    // var result = await _crmService.UpdatePaymentData(model);

        //    return Ok(); // 200
        //}

        [HttpPost]
        public async Task<IActionResult> InitiatePayment(PaymentRequestModel request)
        {
            var apiKey = _config["PaymentGateway:APIKey"];
            var requestUrl = _config["PaymentGateway:RequestUrl"];

            // Serialize the request to JSON
            var jsonPayload = JsonConvert.SerializeObject(request);

            // Compute the SHA512 hash
            var signature = ComputeSha512Hash(jsonPayload + apiKey);

            var client = _httpClientFactory.CreateClient();
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Add("signature", signature);

            var response = await client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var paymentResponse = JsonConvert.DeserializeObject<PaymentResponseModel>(responseContent);
                return Ok(paymentResponse);
            }
            else
            {
                return StatusCode((int)response.StatusCode, responseContent);
            }
        }

        private static string ComputeSha512Hash(string rawData)
        {
            using (var sha512 = SHA512.Create())
            {
                var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            await _paymentService.SaveWebhookResponseAsync(body);
            PaymentResponseDTO dto = JsonConvert.DeserializeObject<PaymentResponseDTO>(body);
            if (dto != null && dto.TxStatus == "SUCCESS")
            {
                var paymentRecord = await _paymentService.GetPaymentByOrderIdAsync(dto.OrderId);
                dto.PaymentId = paymentRecord.Id;
                dto.PaymentTypeId= paymentRecord.PaymentTypeId;
                //update visitor parking payment
                if (paymentRecord.VendorId != null)
                {
                    await _paymentService.SaveVendorPaymentAsync(dto, paymentRecord.VendorId.Value);
                }
                

            }
            else
            {
                _logger.LogInformation("Webhook Response : " + body);
            }
            _logger.LogInformation("Webhook Response : " + body);
            // Log the webhook data
            System.IO.File.AppendAllText("webhook_log.txt", $"Received at {DateTime.UtcNow}: {body}\n");

            // Process the webhook data as needed

            return Ok();
        }


        [AllowAnonymous]
        // POST api/payments/request
        [HttpPost]
        public async Task<IActionResult> RequestTransaction(PaymentRequestDTO req)
        {
             var paymentRequest =await _paymentService.SavePaymentRequestAsync(req);
            // Ensure merchantId is set from config
            req.merchantId = _opts.MerchantId;

            // Save incoming order to store (demo)
            _orderStore.Save(paymentRequest.orderId, new { status = "INIT", request = req });

            // Call Ampersand /tx/request
            var response = await _ampClient.PostAsync<PaymentRequestDTO, JsonElement>("/tx/request", req);

            // return response directly to client
            return Ok(response);
        }

        // POST api/payments/query
        [HttpPost]
        public async Task<IActionResult> Query([FromBody] TransactionQueryDto req)
        {
            req.merchantId = _opts.MerchantId;
            var response = await _ampClient.PostAsync<TransactionQueryDto, JsonElement>("/tx/query", req);
            return Ok(response);
        }

        // POST api/payments/void
        [HttpPost("void")]
        public async Task<IActionResult> Void([FromBody] TransactionVoidDto req)
        {
            req.merchantId = _opts.MerchantId;
            var response = await _ampClient.PostAsync<TransactionVoidDto, JsonElement>("/tx/void", req);
            return Ok(response);
        }

        // GET api/payments/channel
        [HttpGet("channel")]
        public async Task<IActionResult> GetChannelList()
        {
            var bodyForSign = new { merchantId = _opts.MerchantId };
            var response = await _ampClient.GetAsync<JsonElement>("/tx/channel?merchantId=" + _opts.MerchantId, bodyForSign);
            return Ok(response);
        }

        // Notification endpoint to receive callbacks from Ampersand
        // POST api/payments/notification
        [HttpPost("notification")]
        public async Task<IActionResult> Notification()
        {
            // Read raw body
            using var sr = new System.IO.StreamReader(Request.Body);
            var body = await sr.ReadToEndAsync();

            // Signature header
            if (!Request.Headers.TryGetValue("signature", out var providedSig))
                return BadRequest(new { ret = "8031", msg = "Missing signature header" });

            // Verify signature
            if (!SignatureHelper.Verify(body, _opts.SecretKey, providedSig))
                return Unauthorized(new { ret = "8031", msg = "Invalid signature" });

            // parse JSON
            var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // extract orderId and txStatus
            string orderId = root.GetProperty("orderId").GetString();
            string txStatus = root.GetProperty("txStatus").GetString();

            // Update store (demo)
            _orderStore.Update(orderId, new { status = txStatus, notification = root });

            // Respond 200 to Ampersand
            return Ok(new { ret = "0000", msg = "Received" });
        }


        [HttpGet]
        public async Task<IActionResult> getPaymentDetailsByTransactionId(string transactionId)
        {
            var visitor = await _paymentService.getPaymentDetailsByTransactionIdAsync(transactionId);
            if (visitor == null)
                return NotFound();
            return Ok(visitor);
        }


        //[HttpGet]
        //public async Task<IActionResult> CheckUserPaymentStatus(int residentId)
        //{
        //    return Ok(await _paymentService.CheckUserPaymentStatus(residentId));
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetResidentPaymentHistory(int residentId)
        //{
        //    return Ok(await _paymentService.GetResidentPaymentHistoryAsync(residentId));
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetLast30DaysPaymentsByResident(int residentId)
        //{
        //    return Ok(await _paymentService.GetLast30DaysPaymentsByResidentAsync(residentId));
        //}

    }
}
