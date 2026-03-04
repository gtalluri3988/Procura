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
    public class VisitorService : IVisitorService
    {

        private readonly IVisitorRepository _visitorRepository;

        public VisitorService(IVisitorRepository visitorRepository)
        {
            _visitorRepository = visitorRepository;
        }

        public async Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsAsync()
        {
            return await _visitorRepository.GetAllVisitorsAsync();
        }
        public async Task<IEnumerable<VisitorAccessModel>> GetAllCommunityAdminVisitorsAsync()
        {
            return await _visitorRepository.GetAllCommunityAdminVisitorsAsync();

        }
        public async Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsAsync(int communityId)
        {
            return await _visitorRepository.GetAllVisitorsAsync(communityId);
        }

        public async Task<IEnumerable<VisitorAccessDetailsDTO>> GetAllVisitorsByCommunityAsync(int communityId)
        {
            return await _visitorRepository.GetAllVisitorsByCommunityAsync(communityId);
        }

        public async Task<VisitorAccessDetailsDTO> GetVisitorsByIdAsync(int id)
        {
            return await _visitorRepository.GetByIdAsync(id);
        }

        public async Task<VisitorAccessDetailsDTO> GetVisitorsByCommunityResidentIdAsync(int visitorId)
        {
            return await _visitorRepository.GetVisitorsByCommunityIdResidentIdAsync(visitorId);
        }

        public async Task<VisitorAccessDetailsDTO> CreateVisitorAsync(VisitorAccessDetailsDTO dto)
        {
            return await _visitorRepository.AddAsync(dto);
        }

        public async Task<VisitorAccessDetailsDTO> SaveVisitorAsync(VisitorAccessDetailsDTO dto)
        {
            return await _visitorRepository.SaveVisitorDetailsAsync(dto);
        }

        public async Task UpdateVisitorAsync(int id, VisitorAccessDetailsDTO dto)
        {
            await _visitorRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteVisitorAsync(int id)
        {
            await _visitorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsBysearchParams(VisitorAccessDetailsDTO Params)
        {

           return await _visitorRepository.SearchVisitorsByCommunityIdAsync(Params);
        }

        public async Task<VisitorAccessDetailsDTO> SaveVisitorMobileDetailsAsync(VisitorAccessDetailsDTO resident)
        {
            return await _visitorRepository.SaveVisitorMobileDetailsAsync(resident);
        }

        public async Task SendVisitorQREmail(QRImageModel model)
        {
            await _visitorRepository.SendVisitorQREmail(model);

        }

        public async Task<VisitorAccessDetailsDTO> GetVisitorByVehicleNoAsync(string vehicleNo)
        {
            return await _visitorRepository.GetVisitorByVehicleNoAsync(vehicleNo);
        }
    }
}
