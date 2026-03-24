using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IPaymentService
    {
        //Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
        //Task<PaymentDTO> GetPaymentByIdAsync(int residentId);
        //Task<PaymentDTO> CreatePaymentAsync(PaymentDTO dto);
        //Task UpdatePaymentAsync(int id, PaymentDTO dto);
        //Task<double> GetTotalMaintanence(int paymentTypeId, int communityId);

        // Task<double> GetMaintanenceFeeTotalByCommunity(int communityId, int paymentTypeId, bool includeCommunityId = false);
        Task<PaymentRequestDTO> SavePaymentRequestAsync(PaymentRequestDTO dto);
        Task SaveWebhookResponseAsync(string response);
        Task<PaymentRequestDTO> GetPaymentByOrderIdAsync(string orderId);

        Task SaveVendorPaymentAsync(PaymentResponseDTO dto, int vendorId);
        //Task SaveResidentFacilityPaymentAsync(PaymentResponseDTO dto, int facilityId);
        //Task SaveResidentMaintanencePaymentAsync(PaymentResponseDTO dto, int residentId);
        Task<PaymentResponseDTO> getPaymentDetailsByTransactionIdAsync(string transactionId);

        //Task<MaintanancePaymentStatusDTO> CheckUserPaymentStatus(int residentId);

        //Task<List<MaintanancePaymentStatusDTO>> GetResidentPaymentHistoryAsync(int residentId);

        //Task<List<PaymentSummaryDto>> GetLast30DaysPaymentsByResidentAsync(int residentId);

    }
}
