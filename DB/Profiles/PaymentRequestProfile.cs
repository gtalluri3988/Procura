using AutoMapper;
using DB.EFModel;
using DB.Entity;


namespace DB.Profiles
{
    public class PaymentRequestProfile : Profile
    {
        public PaymentRequestProfile()
        {
            CreateMap<PaymentRequest, PaymentRequestDTO>();
            CreateMap<PaymentRequestDTO, PaymentRequest>();
        }
    }
}
