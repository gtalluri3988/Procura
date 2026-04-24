using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IVendorService _vendorService;

        public PaymentService(IPaymentRepository paymentRepository, IVendorService vendorService)
        {
            _paymentRepository = paymentRepository;
            _vendorService = vendorService;
        }
        //public async Task<PaymentDTO> CreatePaymentAsync(PaymentDTO dto)
        //{
        //    return await _paymentRepository.AddPaymentAsync(dto);
        //}

        //public async Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync()
        //{
        //    return await _paymentRepository.GetAllPaymentAsync();
        //}

        //public async Task<PaymentDTO> GetPaymentByIdAsync(int paymentId)
        //{
        //    return await _paymentRepository.GetPaymentByIdAsync(paymentId);
        //}

        //public async Task<double> GetMaintanenceFeeTotalByCommunity(int communityId, int paymentTypeId, bool includeCommunityId = false)
        //{
        //    return await _paymentRepository.GetMaintanenceFeeTotalByPaymentType(communityId,paymentTypeId, includeCommunityId);
        //}

        //public async Task UpdatePaymentAsync(int id, PaymentDTO dto)
        //{
        //    await _paymentRepository.UpdatePaymentAsync(id,dto);
        //}

        public async Task<PaymentRequestDTO> SavePaymentRequestAsync(PaymentRequestDTO dto)
        {
            return await _paymentRepository.SavePaymentRequestAsync(dto);
        }
        public async Task SaveWebhookResponseAsync(string response)
        {
            await _paymentRepository.SaveWebhookResponseAsync(response);
        }
        public async Task<PaymentRequestDTO> GetPaymentByOrderIdAsync(string orderId)
        {
            return await _paymentRepository.GetPaymentByOrderIdAsync(orderId);
        }

        public async Task SaveVendorPaymentAsync(PaymentResponseDTO dto, int vendorId)
        {
            await _paymentRepository.SaveVendorPaymentAsync(dto, vendorId);
            await _vendorService.SendRegistrationConfirmationAsync(vendorId);
        }

        public async Task SaveVendorPaymentMockAsync(int vendorId)
        {
            await _paymentRepository.SaveVendorPaymentMockAsync(vendorId);
            await _vendorService.SendRegistrationConfirmationAsync(vendorId);
        }

        //public async Task SaveResidentFacilityPaymentAsync(PaymentResponseDTO dto, int facilityId)
        //{
        //    await _paymentRepository.SaveResidentFacilityPaymentAsync(dto,  facilityId);
        //}

        //public async Task SaveResidentMaintanencePaymentAsync(PaymentResponseDTO dto, int residentId)
        //{
        //    await _paymentRepository.SaveResidentMaintanencePaymentAsync(dto, residentId);
        //}

        public async Task<PaymentResponseDTO> getPaymentDetailsByTransactionIdAsync(string transactionId)
        {
            return await _paymentRepository.getPaymentDetailsByTransactionIdAsync(transactionId);
        }

        //public async Task<MaintanancePaymentStatusDTO> CheckUserPaymentStatus(int residentId)
        //{
        //    return await _paymentRepository.CheckUserPaymentStatus(residentId);
        //}

        //public async Task<List<MaintanancePaymentStatusDTO>> GetResidentPaymentHistoryAsync(int residentId)
        //{
        //    return await _paymentRepository.GetResidentPaymentHistoryAsync(residentId);
        //}

        //public async Task<List<PaymentSummaryDto>> GetLast30DaysPaymentsByResidentAsync(int residentId)
        //{
        //    return await _paymentRepository.GetLast30DaysPaymentsByResidentAsync(residentId);
        //}
    }
}
