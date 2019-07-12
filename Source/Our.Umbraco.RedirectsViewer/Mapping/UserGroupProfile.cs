using AutoMapper;
using Our.Umbraco.RedirectsViewer.Models;
using Umbraco.Core.Models.Membership;

namespace Our.Umbraco.RedirectsViewer.Mapping
{
    internal class UserGroupProfile:Profile
    {
        public UserGroupProfile()
        {
            CreateMap<IUserGroup, UserGroupDisplay>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Alias, opt => opt.MapFrom(src => src.Alias));
        }
    }
}
