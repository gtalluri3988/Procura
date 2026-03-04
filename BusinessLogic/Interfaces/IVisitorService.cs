using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IVisitorService
    {
        Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsAsync();
        Task<IEnumerable<VisitorAccessDetailsDTO>> GetAllVisitorsByCommunityAsync(int communityId);
        Task<VisitorAccessDetailsDTO> GetVisitorsByIdAsync(int id);
        Task<VisitorAccessDetailsDTO> CreateVisitorAsync(VisitorAccessDetailsDTO dto);
        Task UpdateVisitorAsync(int id, VisitorAccessDetailsDTO dto);
        Task DeleteVisitorAsync(int id);
        Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsBysearchParams(VisitorAccessDetailsDTO Params);
        Task<VisitorAccessDetailsDTO> SaveVisitorAsync(VisitorAccessDetailsDTO dto);

        Task<VisitorAccessDetailsDTO> GetVisitorsByCommunityResidentIdAsync(int visitorId);
        Task<VisitorAccessDetailsDTO> SaveVisitorMobileDetailsAsync(VisitorAccessDetailsDTO resident);

        Task SendVisitorQREmail(QRImageModel model);
        Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsAsync(int communityId);

        Task<IEnumerable<VisitorAccessModel>> GetAllCommunityAdminVisitorsAsync();

        Task<VisitorAccessDetailsDTO> GetVisitorByVehicleNoAsync(string vehicleNo);
    }
}
