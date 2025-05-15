using AutoMapper;
using portal_agile.Dtos.Permissions;
using portal_agile.Security;

namespace portal_agile.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            // Permission and Permission Create DTO mapping
            CreateMap<Permission, PermissionCreateDto>();
        }
    }
}
