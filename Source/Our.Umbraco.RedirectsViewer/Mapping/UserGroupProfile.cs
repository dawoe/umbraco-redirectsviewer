using System;
using System.Collections.Concurrent;
using Our.Umbraco.RedirectsViewer.Models;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Mappers;

namespace Our.Umbraco.RedirectsViewer.Mapping
{

    public sealed class UserGroupProfile: BaseMapper
    {

        //public void DefineMaps(UmbracoMapper mapper)
        //{
        //    mapper.Define<IUserGroup,UserGroupDisplay>(
        //        (source, context) => new UserGroupDisplay()
        //        ,
        //        (source, target, context) =>
        //        {
        //            target.Name = source.Name;
        //            target.Alias = source.Alias;
        //        }   
        //    );
        //}

        public UserGroupProfile(Lazy<ISqlContext> sqlContext, ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> maps) : base(sqlContext, maps)
        {
        }

        protected override void DefineMaps()
        {
            DefineMap<IUserGroup, UserGroupDisplay>(nameof(IUserGroup.Name), nameof(UserGroupDisplay.Name));
            DefineMap<IUserGroup, UserGroupDisplay>(nameof(IUserGroup.Alias), nameof(UserGroupDisplay.Alias));
        }
    }
}
