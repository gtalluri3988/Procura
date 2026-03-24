using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Helper;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace DB.Repositories
{
    public class PaymentRepository : RepositoryBase<User, PaymentDTO>, IPaymentRepository
    {


        public PaymentRepository(ProcuraDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) :
            base(context, mapper, httpContextAccessor)
        {


        }

        //public async Task<PaymentDTO> AddPaymentAsync(PaymentDTO dto)
        //{
        //    var entity = _mapper.Map<EFModel.ResidencePaymentDetails>(dto);
        //    _context.ResidencePaymentDetails.Add(entity);
        //    await _context.SaveChangesAsync();
        //    return await GetByIdAsync(entity.Id);
        //}

        //public async Task<IEnumerable<PaymentDTO>> GetAllPaymentAsync()
        //{
        //    var Payments = await _context.ResidencePaymentDetails.Include(c => c.PaymentStatus).Include(c => c.PaymentType).Include(c => c.resident).ToListAsync();
        //    return _mapper.Map<IEnumerable<PaymentDTO>>(Payments);
        //}

        //public async Task<PaymentDTO> GetPaymentByIdAsync(int id)
        //{
        //    var residents = await _context.ResidencePaymentDetails.Where(x => x.Id == id).Include(c => c.PaymentStatus).Include(c => c.PaymentType).Include(c => c.resident).FirstOrDefaultAsync();
        //    return _mapper.Map<PaymentDTO>(residents);
        //}

        //public async Task UpdatePaymentAsync(int payId, PaymentDTO payment)
        //{
        //    var entity = await _context.ResidencePaymentDetails.FirstOrDefaultAsync(c => c.Id == payId);

        //    if (entity != null && payment != null)
        //    {
        //        //entity.PaymentStatus = payment.PaymentStatus;
        //        //entity.PaymentDate = role.Status;
        //        //entity.UpdatedDate = DateTime.Now;
        //        entity.Amount = payment.Amount;
        //    }
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //public async Task<double> GetMaintanenceFeeTotalByPaymentType(int communityId, int paymentTypeId, bool includeCommunityId = false)
        //{

        //    if (includeCommunityId)
        //    {
        //        var totalAmounts = await _context.ResidencePaymentDetails
        //                     .Where(x => x.resident != null && x.resident.CommunityId == communityId && x.PaymentTypeId == paymentTypeId)
        //                     .SumAsync(x => x.Amount);
        //        return totalAmounts;
        //    }
        //    else
        //    {
        //        var totalAmounts = await _context.ResidencePaymentDetails
        //                    .Where(x => x.resident != null && x.PaymentTypeId == paymentTypeId)
        //                    .SumAsync(x => x.Amount);
        //        return totalAmounts;
        //    }
        //}


        public async Task<PaymentRequestDTO> SavePaymentRequestAsync(PaymentRequestDTO dto)
        {
            var entity = new PaymentRequest
            {
                // Order / Invoice
                OrderId = GenerateOrderId(),
                
                InvoiceDesc = dto.orderRef,
                ROCNumber= dto.ROCNumber,

                // Payment
                Amount = dto.txAmount,
                PaymentMethod = dto.txChannel,
                ApiOperation = dto.txType,

                // Customer
                Email = dto.custEmail,

                // Foreign Keys
              
                PaymentTypeId = dto.PaymentTypeId,
                
                // Audit
                CreatedDate = DateTime.UtcNow
            };

            _context.PaymentRequest.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
            dto.Id = entity.ID;
            dto.orderId = entity.OrderId;
            return dto;
        }

        private string GenerateOrderId()
        {
            var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var rnd = new Random().Next(1000, 9999);

            return $"order_{ts}_{rnd}";
        }


        public string GenerateInvoiceNumber(int? communityId)
        {
            var lastNo = _context.PaymentRequest
                        .Where(x => x.Invoice != null)
                        .AsEnumerable()
                        .Select(x =>
                            int.TryParse(
                                x.Invoice.Substring(x.Invoice.Length - 3),
                                out var n) ? n : 0
                        )
                        .DefaultIfEmpty(0)
                        .Max();
            string runningNo = $"{(lastNo + 1):D3}";
            //var community = _context.Community.Find(communityId);
            
                string invoiceNumber = "INV" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + runningNo;
                return invoiceNumber;
            
            return "INV0000";
        }

        //public async Task SaveWebhookResponseAsync(string response)
        //{
        //    var entity = new WebHookResponse
        //    {
        //        // Order / Invoice
        //        Response = response,
        //        ResponseDateTime = DateTime.Now,
        //        CreatedDate = DateTime.Now,
        //    };
        //    _context.WebHookResponses.Add(entity);
        //    await _context.SaveChangesAsync();
        //}



        public async Task SaveWebhookResponseAsync(string response)
        {
            // Deserialize webhook JSON
            var paymentResponse = JsonSerializer.Deserialize<PaymentTransactionResponse>(
            response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var paymentId = _context.PaymentRequest.Where(x => x.OrderId == paymentResponse.OrderId).Select(x => x.ID).FirstOrDefault();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1️⃣ Save raw webhook response
                var webhookEntity = new WebHookResponse
                {
                    Response = response,
                    ResponseDateTime = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };

                _context.WebHookResponses.Add(webhookEntity);

                // 2️⃣ Save parsed payment transaction
                if (paymentResponse != null)
                {
                    var paymentEntity = new PaymentTransactionResponse
                    {
                        Ret = paymentResponse.Ret,
                        Msg = paymentResponse.Msg,
                        MerchantId = paymentResponse.MerchantId,
                        OrderId = paymentResponse.OrderId,
                        TxId = paymentResponse.TxId,
                        TxType = paymentResponse.TxType,
                        TxStatus = paymentResponse.TxStatus,
                        TxDt = paymentResponse.TxDt,
                        TxAmount = paymentResponse.TxAmount,
                        TxCurrency = paymentResponse.TxCurrency,
                        TxChannel = paymentResponse.TxChannel,
                        RespCode = paymentResponse.RespCode,
                        SchemeId = paymentResponse.SchemeId,
                        PaymentId = paymentId,
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.PaymentTransactionResponses.Add(paymentEntity);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<PaymentRequestDTO> GetPaymentByOrderIdAsync(string orderId)
        {
            var paymentRequest = await _context.PaymentRequest.Where(x => x.OrderId == orderId)
                .FirstOrDefaultAsync();
            return _mapper.Map<PaymentRequestDTO>(paymentRequest);


        }


        public async Task SaveVendorPaymentAsync(PaymentResponseDTO dto, int vendorId)
        {
            var entity = new VendorPaymentStatus
            {
                // Order / Invoice
                VendorId = vendorId,
                PaymentId = dto.PaymentId,
                PaymentTypeId= dto.PaymentTypeId,
                PaymentStatus = dto.TxStatus == "SUCCESS" ? "Paid" : "Pending",
                // Audit
                CreatedDate = DateTime.UtcNow
            };
            _context.VendorPaymentStatus.Add(entity);
            await _context.SaveChangesAsync();
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor != null)
            {
                vendor.IsRegistrationComplete = true;
                vendor.VendorCodeStatus = VendorStatus.PendingRequest.GetDisplayName();
                await _context.SaveChangesAsync();
            }
        }

        //public async Task SaveResidentFacilityPaymentAsync(PaymentResponseDTO dto, int facilityId)
        //{
        //    var entity = new FacilityPaymentStatus
        //    {
        //        // Order / Invoice
        //        FacilityBookingId = facilityId,
        //        PaymentId = dto.PaymentId,
        //        PaymentStatus = dto.TxStatus == "SUCCESS" ? "Paid" : "Pending",
        //        // Audit
        //        CreatedDate = DateTime.UtcNow
        //    };
        //    _context.FacilityPaymentStatus.Add(entity);
        //    await _context.SaveChangesAsync();
        //}


        //public async Task SaveResidentMaintanencePaymentAsync(PaymentResponseDTO dto, int residentId)
        //{
        //    var payment = await _context.MaintanencePaymentStatus
        //        .Where(x => x.ResidentId == residentId && x.PaymentStatus=="Pending")
        //        .FirstOrDefaultAsync();
        //    if(payment != null)
        //    {
        //        payment.PaymentId = dto.PaymentId;
        //        payment.PaymentStatus = dto.TxStatus == "SUCCESS" ? "Paid" : "Pending";
        //        payment.UpdatedDate = DateTime.Now;
        //        await _context.SaveChangesAsync();
        //    }
        //}

        public async Task<PaymentResponseDTO> getPaymentDetailsByTransactionIdAsync(string transactionId)
        {
            var paymentDetails = await _context.PaymentTransactionResponses
                .Where(x => x.TxId == transactionId)
                .FirstOrDefaultAsync();
            return _mapper.Map<PaymentResponseDTO>(paymentDetails);
        }



        //public async Task<MaintanancePaymentStatusDTO> CheckUserPaymentStatus(int residentId)
        //{
        //    var paymentDetails = await _context.MaintanencePaymentStatus
        //        .Where(x => x.ResidentId == residentId && x.MaintananceYear==DateTime.Now.Year && x.MaintananceMonth==DateTime.Now.Month)
        //        .FirstOrDefaultAsync();
        //    return _mapper.Map<MaintanancePaymentStatusDTO>(paymentDetails);
        //}

        //public async Task<List<MaintanancePaymentStatusDTO>> GetResidentPaymentHistoryAsync(int residentId)
        //{
        //    var paymentDetails = await _context.MaintanencePaymentStatus
        //        .Where(x => x.ResidentId == residentId)
        //        .ToListAsync();
        //    return _mapper.Map<List<MaintanancePaymentStatusDTO>>(paymentDetails);
        //}

        //public async Task<List<PaymentSummaryDto>> GetLast30DaysPaymentsByResidentAsync(int residentId)
        //{
        //    var fromDate = DateTime.UtcNow.AddDays(-30);

        //    var query =
        //        from pr in _context.PaymentRequest
        //        where pr.CreatedDate >= fromDate
        //              && pr.ResidentId == residentId

        //        // LEFT join maintenance first
        //        join mps in _context.MaintanencePaymentStatus
        //            on pr.ID equals mps.PaymentId into mpsJoin
        //        from mps in mpsJoin.DefaultIfEmpty()

        //        join vps in _context.VisitorPaymentStatus
        //            on pr.ID equals vps.PaymentId into vpsJoin
        //        from vps in vpsJoin.DefaultIfEmpty()

        //        join fps in _context.FacilityPaymentStatus
        //            on pr.ID equals fps.PaymentId into fpsJoin
        //        from fps in fpsJoin.DefaultIfEmpty()

        //            // 🔑 CRITICAL FILTER
        //        where
        //            mps != null               // valid maintenance payment
        //            || vps != null            // OR valid visitor payment
        //            || fps != null            // OR valid facility payment

        //        select new PaymentSummaryDto
        //        {
        //            PaymentRequestId = pr.ID,
        //            Amount = pr.Amount,
        //            CreatedDate = pr.CreatedDate,
        //            Invoice=pr.Invoice,

        //            PaymentType =
        //                mps != null ? "Maintenance" :
        //                vps != null ? "Visitor" :
        //                fps != null ? "Facility" :
        //                "Unknown",

        //            PaymentStatus =
        //                mps != null ? mps.PaymentStatus :
        //                vps != null ? vps.PaymentStatus :
        //                fps != null ? fps.PaymentStatus :
        //                "Pending"
        //        };

        //    return await query
        //        .OrderByDescending(x => x.CreatedDate)
        //        .ToListAsync();
        //}


        


    }
}
