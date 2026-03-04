using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Repositories.Interfaces;

namespace BusinessLogic.Services
{
    public class DropDownService:IDropDownService
    {
        private readonly IDropdownRepository _dropdownRepository;
        public DropDownService(IDropdownRepository dropdownRepository)
        {
            _dropdownRepository = dropdownRepository;
        }
        public async Task<List<DropdownItem>> GetSelectList(string inputType)
        {
            var selectList =await _dropdownRepository.GetDropdownDataAsync(inputType);
            return selectList;
        }
    }

    public class dropdownDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
