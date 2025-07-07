using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.MenuItem;
using portal_agile.Dtos.Modules;
using portal_agile.Dtos.Users.Response;
using System.Security.Claims;

namespace portal_agile.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IUserService _userService;
        private readonly ILogger<MenuController> _logger;

        public MenuController(
            IMenuService menuService, 
            IUserService userService,
            ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get complete user menu structure with modules and permissions
        /// </summary>
        [HttpGet("user-menu")]
        public async Task<ActionResult<UserMenuResponse>> GetUserMenu()
        {
            try
            {
                var userId = GetCurrentUserId();
                var userMenu = await _menuService.GetUserMenuAsync(userId);

                _logger.LogInformation("User menu retrieved successfully for user {UserId}", userId);

                return Ok(userMenu);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user menu");
                return StatusCode(500, new { message = "An error occurred while retrieving the menu" });
            }
        }

        private string GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userIdClaim ?? string.Empty;

            throw new UnauthorizedAccessException("User ID not found in claims");
        }

        private async Task<List<string>> GetUserPermissions(string userId)
        {
             return await _userService.GetAllUserPermissionsInStringAsync(userId);
        }
    }

    // Extension methods for cleaner menu handling
    public static class MenuExtensions
    {
        public static List<MenuItemDto> GetMainMenuItems(this List<MenuItemDto> menu)
        {
            return menu.Where(m => m.MenuLevel == 1).ToList();
        }

        public static List<MenuItemDto> GetSubMenuItems(this List<MenuItemDto> menu, int parentId)
        {
            return menu.Where(m => m.ParentId == parentId).ToList();
        }
    }
}
