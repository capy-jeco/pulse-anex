using AutoMapper;
using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Roles;
using portal_agile.Dtos.Users;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            #region Permissions DTO mapping
            CreateMap<Permission, PermissionDto>();
            CreateMap<Permission, PermissionCreateDto>();
            CreateMap<PermissionCreateDto, Permission>();
            CreateMap<Dictionary<string, List<Permission>>, Dictionary<string, List<PermissionDto>>>()
                .ConvertUsing((src, dest, context) =>
                {
                    dest = dest ?? new Dictionary<string, List<PermissionDto>>();

                    foreach (var kvp in src)
                    {
                        var permissionDtoList = context.Mapper.Map<List<PermissionDto>>(kvp.Value);
                        dest[kvp.Key] = permissionDtoList;
                    }

                    return dest;
                });
            #endregion

            #region Roles DTO mapping
            CreateMap<Role, RoleDto>();
            CreateMap<Role, RoleCreateDto>();
            CreateMap<RoleCreateDto, Role>();
            #endregion

            #region Users DTO mapping
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            #endregion
        }
    }
}
