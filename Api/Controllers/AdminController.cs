using BusinessLogic.Interfaces;
using BusinessLogic.Models.Users;
using BusinessLogic.Services;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Services;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : AuthorizedCSABaseAPIController
    {
        private readonly IPasswordPolicyService _passwordPolicyService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;
        private readonly IRoleMenuPermissionService _roleMenuPermissionervice;

        public AdminController(
            IPasswordPolicyService passwordPolicyService,IRoleService roleService,
            ICurrentUserService currentUserService,
            IUserService userService,IMenuService menuService,
            IRoleMenuPermissionService roleMenuPermissionService,
            ILogger<AdminController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _passwordPolicyService = passwordPolicyService;
            _roleService = roleService;
            _roleMenuPermissionervice = roleMenuPermissionService;
            _menuService = menuService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllPasswordPolicyDetails()
        {
            return Ok(await _passwordPolicyService.GetAllPasswordPolicyAsync());
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePasswordPolicy(List<PasswordPolicyDTO> policy)
        {
            return Ok(await _passwordPolicyService.SavePasswordPolicyAsync(policy));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            return Ok(await _roleService.GetAllRolesAsync());
        }
        [HttpGet]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            return Ok(await _roleService.GetRoleByIdAsync(roleId));
        }

        [HttpPost]
        public async Task<IActionResult> SaveRole(RoleDTO role)
        {
            await _roleService.SaveRoleAsync(role);
            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int roleId,RoleDTO role)
        {
            await _roleService.UpdateRoleAsync(roleId,role);
            return Ok(true);
        }



        //Menu

        [HttpGet]
        public async Task<IActionResult> GetAllMenuRolePermission()
        {
            return Ok(await _roleMenuPermissionervice.GetAllMenuRolesAsync());
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveMenuRolePermission(RoleMenuPermissionDTO roleMenuPermissionDTO)
        {
            try
            {
                var createdUser = await _roleMenuPermissionervice.CreateMenuRoleAsync(roleMenuPermissionDTO);
                return Ok(new { message = "Role Permissions created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Return only the message
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoleMenuPermission(int id, RoleMenuPermissionDTO roleMenuPermissionDTO)
        {
            try
            {
                await _roleMenuPermissionervice.UpdateMenuRoleAsync(id, roleMenuPermissionDTO);
                return Ok(new { message = "Role Permissions created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Return only the message
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMenu()
        {
            return Ok(await _menuService.GetAllMenusAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetSubMenuListByMenuId(int subMenuId)
        {
            return Ok(await _menuService.GetSubMenuList(subMenuId));
        }
        

        [HttpGet]
        public async Task<IActionResult> GetAllMenusByRole(int roleId)
        {
            return Ok(await _roleMenuPermissionervice.GetAllMenusByRolesAsync(roleId));
        }

    }
}
