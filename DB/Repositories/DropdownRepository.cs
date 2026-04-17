
using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Helper;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DB.Repositories
{
    public class DropdownRepository : RepositoryBase<DropDownDTO, DropdownItem>, IDropdownRepository
    {
        public DropdownRepository(ProcuraDbContext context, IMapper mapper,IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }
        public async Task<List<DropdownItem>?> GetDropdownDataAsync(string inputType)
        {
            if (inputType.ToLower() == DropDown.PaymentChannel.ToString().ToLower())
            {
                return _context.PaymentChannel.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }

            if (inputType.ToLower() == DropDown.State.ToString().ToLower())
            {
                return _context.State.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.VendorType.ToString().ToLower())
            {
                return _context.CompanyEntityTypes.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.Country.ToString().ToLower())
            {
                return _context.Countries.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.Equals(DropDown.RegistrationStatus.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return Enum.GetValues(typeof(VendorStatus))
                    .Cast<VendorStatus>()
                    .Select(e => new DropdownItem
                    {
                        Id = (int)e,
                        Name = EnumExtensions.GetDisplayName(e)
                    })
                    .ToList();
            }
            if (inputType == DropDown.Status.ToString())
            {
                return new()
                {
                    new DropdownItem { Id = 1, Name = "Active" },
                    new DropdownItem { Id = 0, Name = "Inactive" }
                };

            }
            if (inputType.ToLower() == DropDown.Role.ToString().ToLower())
            {
                return _context.Roles.Where(x=>x.Status=="1" && x.Name != "ResidentUser").Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.Menu.ToString().ToLower())
            {
                return _context.Menus.Where(x=>x.ParentId=="0").Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.Questionaire.ToString().ToLower())
            {
                return _context.Questionnaires.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.SiteLevel.ToString().ToLower())
            {
                return _context.SiteLevel.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.Designation.ToString().ToLower())
            {
                return _context.Designations.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.JobCategory.ToString().ToLower())
            {
                return _context.JobCategories.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.TenderCategory.ToString().ToLower())
            {
                return _context.TenderCategories.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.TenderApplicationStatus.ToString().ToLower())
            {
                return _context.TenderApplicationStatus.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }
            if (inputType.ToLower() == DropDown.BankKey.ToString().ToLower())
            {
                return _context.BankKeys.Select(item => new DropdownItem
                {
                    Id = item.Id,
                    Name = item.BankName ?? string.Empty,
                }).ToList() ?? new List<DropdownItem>();
            }

            return new List<DropdownItem>();

        }

    }


   

}
