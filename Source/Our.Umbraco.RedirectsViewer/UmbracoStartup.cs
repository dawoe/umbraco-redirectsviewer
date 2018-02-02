namespace Our.Umbraco.RedirectsViewer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using global::Umbraco.Core;
    using global::Umbraco.Web;
    using global::Umbraco.Web.UI.JavaScript;

    using Our.Umbraco.RedirectsViewer.Controllers;

    /// <summary>
    /// The umbraco startup handler
    /// </summary>
    internal class UmbracoStartup : ApplicationEventHandler
    {
        /// <summary>
        /// Overridable method to execute when Bootup is completed, this allows you to perform any other bootup logic required for the application.
        /// Resolution is frozen so now they can be used to resolve instances.
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // setup server variables
            ServerVariablesParser.Parsing += this.ServerVariablesParserParsing;
        }

        /// <summary>
        /// ServerVariablesParser parsing event handler
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ServerVariablesParserParsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            var urlDictionairy = new Dictionary<string, object>();

            urlDictionairy.Add("UserGroupApi", urlHelper.GetUmbracoApiServiceBaseUrl<UserGroupsApiController>(c => c.GetUserGroups()));
            
            if (!e.Keys.Contains("Our.Umbraco.RedirectsViewer"))
            {
                e.Add("Our.Umbraco.RedirectsViewer", urlDictionairy);
            }
        }
    }
}
