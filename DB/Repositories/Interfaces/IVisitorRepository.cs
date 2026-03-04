using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IVisitorRepository
    {

        Task<IEnumerable<VisitorAccessDetailsDTO>> GetAllAsync();
        Task<VisitorAccessDetailsDTO> GetByIdAsync(int id);
        Task<VisitorAccessDetailsDTO> AddAsync(VisitorAccessDetailsDTO dto);
        Task UpdateAsync(int id, VisitorAccessDetailsDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsAsync();
        Task<IEnumerable<VisitorAccessDetailsDTO>> GetAllVisitorsByCommunityAsync(int communityId);
        Task<IEnumerable<VisitorAccessModel>> SearchVisitorsByCommunityIdAsync(VisitorAccessDetailsDTO searchModel);
        Task<VisitorAccessDetailsDTO> SaveVisitorDetailsAsync(VisitorAccessDetailsDTO resident);
        Task<VisitorAccessDetailsDTO> GetVisitorsByCommunityIdResidentIdAsync(int visitorId);
        Task<VisitorAccessDetailsDTO> SaveVisitorMobileDetailsAsync(VisitorAccessDetailsDTO resident);
        Task SendVisitorQREmail(QRImageModel model);
         Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsAsync(int communityId);
        Task<IEnumerable<VisitorAccessModel>> GetAllCommunityAdminVisitorsAsync();

        Task<VisitorAccessDetailsDTO> GetVisitorByVehicleNoAsync(string vehicleNo);


    }
}
