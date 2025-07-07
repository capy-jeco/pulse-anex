using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Services;
using portal_agile.Data;
using portal_agile.Dtos.MenuItem;
using portal_agile.Dtos.Modules;
using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Users.Response;
using portal_agile.Models;
using System.Linq;

namespace portal_agile.Services
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService; // Your existing user service
        private static readonly Dictionary<string, string> _moduleRoutes = new()
        {
            { "usermanagement", "/users" },
            { "rolemanagement", "/roles" },
            { "employeemanagement", "/employees" },
            { "departmentmanagement", "/departments" },
            { "reports", "/reports" },
            { "settings", "/settings" }
        };

        public MenuService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<UserMenuResponse> GetUserMenuAsync(string userId)
        {
            var userPermissions = await _userService.GetAllUserPermissionsInStringAsync(userId);
            var userPermissionSet = userPermissions.ToHashSet();

            var modules = await GetModulesWithPermissionsAsync(userPermissionSet);
            var menu = await GetMenuStructureAsync(userPermissionSet);

            return new UserMenuResponse
            {
                Modules = modules,
                Menu = menu,
                UserPermissions = userPermissions
            };
        }

        private async Task<List<MenuItemDto>> GetMenuStructureAsync(HashSet<string> userPermissions)
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.Module)
                .Where(m => m.IsVisible && m.IsActive)
                .Where(m => string.IsNullOrEmpty(m.RequiredPermission) || userPermissions.Contains(m.RequiredPermission))
                .OrderBy(m => m.MenuLevel)
                .ThenBy(m => m.SortOrder)
                .ToListAsync();

            return BuildMenuTree(menuItems, userPermissions);
        }

        private async Task<List<ModulePermissionDto>> GetModulesWithPermissionsAsync(HashSet<string> userPermissions)
        {
            // Single optimized query with joins
            var modulePermissions = await (
                from module in _context.Modules
                where module.IsActive
                join menuItem in _context.MenuItems on module.Id equals menuItem.ModuleId
                where menuItem.IsActive && menuItem.IsVisible
                join mip in _context.MenuItemPermissions on menuItem.Id equals mip.MenuItemId
                join permission in _context.Permissions on mip.PermissionId equals permission.PermissionId
                where userPermissions.Contains(permission.Code)
                select new
                {
                    ModuleName = module.ModuleName,
                    DisplayName = module.DisplayName,
                    PermissionId = permission.PermissionId,
                    PermissionName = permission.Name,
                    PermissionCode = permission.Code,
                    PermissionDescription = permission.Description
                }
            ).ToListAsync();

            // Group and transform results
            return modulePermissions
                .GroupBy(x => new { x.ModuleName, x.DisplayName })
                .Select(g => new ModulePermissionDto
                {
                    ModuleName = g.Key.ModuleName,
                    DisplayName = g.Key.DisplayName,
                    Route = GetModuleRoute(g.Key.ModuleName),
                    Permissions = g.GroupBy(p => p.PermissionCode)
                                 .Select(pg => pg.First())
                                 .Select(p => new PermissionDto
                                 {
                                     PermissionId = p.PermissionId,
                                     Name = p.PermissionName,
                                     Code = p.PermissionCode,
                                     ModuleName = g.Key.ModuleName,
                                     Description = p.PermissionDescription
                                 })
                                 .ToList()
                })
                .ToList();
        }

        private List<MenuItemDto> BuildMenuTree(List<MenuItem> menuItems, HashSet<string> userPermissions)
        {
            // Create lookup dictionaries for O(1) access
            var menuDict = menuItems.ToDictionary(m => m.Id, m => new MenuItemDto
            {
                Id = m.Id,
                Label = m.Label,
                Icon = m.Icon ?? string.Empty,
                Route = m.Route ?? string.Empty,
                ParentId = m.ParentId,
                MenuLevel = m.MenuLevel,
                RequiredPermission = m.RequiredPermission,
                Tooltip = m.Tooltip ?? string.Empty,
                ModuleName = m.Module?.ModuleName ?? string.Empty,
                SortOrder = m.SortOrder,
                Children = new List<MenuItemDto>()
            });

            var rootItems = new List<MenuItemDto>();

            // Build tree structure in single pass
            foreach (var item in menuDict.Values)
            {
                if (item.ParentId.HasValue && menuDict.TryGetValue(item.ParentId.Value, out var parent))
                {
                    parent.Children.Add(item);
                }
                else
                {
                    rootItems.Add(item);
                }
            }

            return FilterMenuItems(rootItems, userPermissions);
        }

        private List<MenuItemDto> FilterMenuItems(List<MenuItemDto> items, HashSet<string> userPermissions)
        {
            var result = new List<MenuItemDto>();

            foreach (var item in items)
            {
                var hasPermission = string.IsNullOrEmpty(item.RequiredPermission) ||
                                   userPermissions.Contains(item.RequiredPermission);

                if (item.Children.Any())
                {
                    item.Children = FilterMenuItems(item.Children, userPermissions);

                    if (item.Children.Any() || hasPermission)
                    {
                        result.Add(item);
                    }
                }
                else if (hasPermission)
                {
                    result.Add(item);
                }
            }

            return result.OrderBy(x => x.SortOrder).ToList();
        }

        private string GetModuleRoute(string moduleName)
        {
            var lowerModuleName = moduleName.ToLower();
            return _moduleRoutes.TryGetValue(lowerModuleName, out var route)
                ? route
                : $"/{lowerModuleName}";
        }
    }
}
