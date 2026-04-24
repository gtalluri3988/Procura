using Api.Models;
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
        private readonly IEmailService _emailService;

        public AdminController(
            IPasswordPolicyService passwordPolicyService,IRoleService roleService,
            ICurrentUserService currentUserService,
            IUserService userService,IMenuService menuService,
            IRoleMenuPermissionService roleMenuPermissionService,
            IEmailService emailService,
            ILogger<AdminController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _passwordPolicyService = passwordPolicyService;
            _roleService = roleService;
            _roleMenuPermissionervice = roleMenuPermissionService;
            _menuService = menuService;
            _emailService = emailService;
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

        [HttpPost]
        public async Task<ActionResult<CSAResponseModel<bool>>> DeleteRole(int roleId)
        {
            try
            {
                if (roleId <= 0)
                {
                    return BadRequest(new CSAResponseModel<bool>(true, "A valid role id is required."));
                }

                var deleted = await _roleService.DeleteRoleAsync(roleId);
                if (!deleted)
                {
                    return NotFound(new CSAResponseModel<bool>(true, "Role not found."));
                }

                return Ok(new CSAResponseModel<bool>(true));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new CSAResponseModel<bool>(true, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", roleId);
                return BadRequest(new CSAResponseModel<bool>(true, "Unable to delete role."));
            }
        }



        //Menu

        [HttpGet]
        public async Task<IActionResult> GetAllMenuRolePermission()
        {
            return Ok(await _roleMenuPermissionervice.GetAllMenuRolesAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetRolePermissionById(int permissionId)
        {
            return Ok(await _roleMenuPermissionervice.GetRolePermissionAsync(permissionId));
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

        [HttpPost]
        public async Task<ActionResult<CSAResponseModel<bool>>> DeleteRoleMenuPermission(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new CSAResponseModel<bool>(true, "A valid permission id is required."));
                }

                var deleted = await _roleMenuPermissionervice.DeleteMenuRoleAsync(id);
                if (!deleted)
                {
                    return NotFound(new CSAResponseModel<bool>(true, "Role permission not found."));
                }

                return Ok(new CSAResponseModel<bool>(true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role permission {PermissionId}", id);
                return BadRequest(new CSAResponseModel<bool>(true, ex.Message));
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<CSAResponseModel<int>>> ProcessEmailQueue(int batchSize = 50)
        {
            try
            {
                if (batchSize <= 0) batchSize = 50;
                if (batchSize > 500) batchSize = 500;

                var processed = await _emailService.ProcessQueueAsync(batchSize);
                _logger.LogInformation("ProcessEmailQueue manual trigger: processed {Count} messages.", processed);
                return Ok(new CSAResponseModel<int>(processed));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessEmailQueue manual trigger failed.");
                return BadRequest(new CSAResponseModel<int>(true, ex.Message));
            }
        }

    }
}
