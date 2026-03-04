using AutoMapper;
using DB.EFModel;
using DB.Entity;


namespace DB.Profiles
{
    public class PaymentResponseProfile : Profile
    {
        public PaymentResponseProfile()
        {
            CreateMap<PaymentTransactionResponse, PaymentResponseDTO>();

            CreateMap<PaymentResponseDTO, PaymentTransactionResponse>();
        }
    }
}

