namespace Our.Umbraco.RedirectsViewer.Mapping
{
    using AutoMapper;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models.Mapping;
    using global::Umbraco.Core.Models.Membership;

    using Our.Umbraco.RedirectsViewer.Models;

    /// <summary>
    /// The usergroup mapping.
    /// </summary>
    internal class UsergroupMapping : MapperConfiguration
    {
        /// <summary>
        /// Configures the user group mappings
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        public override void ConfigureMappings(IConfiguration config, ApplicationContext applicationContext)
        {
            config.CreateMap<IUserType, UserGroupDisplay>();
        }
    }
}
