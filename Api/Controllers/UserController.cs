using Api.Models;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Procura.Models;
using System.Security.Claims;
using YourNamespace.Services;
[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;
    public UserController(ICurrentUserService currentUserService,IUserService userService)
    {
        _currentUserService = currentUserService;
        _userService= userService;
    }
    [HttpGet("current")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Extract User ID
        var username = User.Identity?.Name; // Extract Username
        var email = User.FindFirst(ClaimTypes.Email)?.Value; // Extract Email
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList(); // Extract Roles
        return Ok(new
        {
            UserId = userId,
            Username = username,
            Email = email,
            Roles = roles
        });
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await _userService.GetUserListAsync());
    }
    [HttpGet]
    public async Task<IActionResult> GetBiddingUsersList()
    {
        return Ok(await _userService.GetBiddingUsersList());
    }

    [HttpPost]
    public async Task<IActionResult> GetAllUsers(UserSearch userSearch)
    {
        return Ok(await _userService.GetUserListAsync(userSearch.siteLevelId,userSearch.siteOfficeId,userSearch.status));
    }

    [HttpPost]
    public async Task<IActionResult> GetBiddingUsersList(UserSearch userSearch)
    {
        return Ok(await _userService.GetBidderUserListAsync(userSearch.siteLevelId, userSearch.siteOfficeId, userSearch.status));
    }


    [HttpPost]
    public async Task<IActionResult> CreateUser(UserDTO user)
    {
        try
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return Ok(new { message = "User created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // Return only the message
        }
    }
    [HttpPost("UpdateUser/{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserDTO dto)
    {
        await _userService.UpdateUserAsync(id, dto);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserByAdmin(UserDTO dto)
    {
        await _userService.UpdateUserAsync(dto.Id, dto);
        return NoContent();
    }
    [HttpGet]
    public async Task<IActionResult> GetUserById(int userId)
    {
        return Ok(await _userService.GetUserByIdAsync(userId));
    }

    [HttpPost]
    public async Task<ActionResult<CSAResponseModel<string>>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new CSAResponseModel<string>(true, "Request payload is required."));

            var userIdClaim = HttpContext.User.FindFirst("userid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
                return Unauthorized(new CSAResponseModel<string>(true, "Invalid or missing user identity."));

            await _userService.ChangePasswordAsync(userId, dto);
            return Ok(new CSAResponseModel<string>("Password changed successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new CSAResponseModel<string>(true, ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(new CSAResponseModel<string>(true, ex.Message));
        }
    }
}
