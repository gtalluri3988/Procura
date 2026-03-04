using BusinessLogic.Models;
using DB.Entity.SAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ISAPServices
    {
        Task<VendorCreateResponse> CreateVendorSAP(VendorCreateRequest_SAP_Dto request);

        Task<VendorChangeResponse> ChangeVendorSAP(VendorChangeRequest_SAP_Dto request);
    }
}
