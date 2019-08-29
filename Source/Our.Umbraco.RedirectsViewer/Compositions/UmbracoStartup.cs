﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Our.Umbraco.RedirectsViewer.Controllers;
using Our.Umbraco.RedirectsViewer.Models.Import;
using Our.Umbraco.RedirectsViewer.Services;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.RedirectsViewer.Compositions
{
    public class UmbracoStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;

            composition.RegisterAuto(typeof(RedirectService));
            composition.RegisterUnique<IRedirectPublishedContentFinder>(factory =>
            {
                var umbCtx = (IUmbracoContextAccessor)factory.GetInstance(typeof(IUmbracoContextAccessor));
               return new RedirectPublishedContentFinder(umbCtx.UmbracoContext.Content);
            });


        }

        private void ServerVariablesParser_Parsing(object sender, System.Collections.Generic.Dictionary<string, object> e)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            SetUpDictionaryForAngularPropertyEditor(e);
        }

        private static void SetUpDictionaryForAngularPropertyEditor(Dictionary<string, object> e)
        {
            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            var urlDictionairy = new Dictionary<string, object>
            {
                {
                    "UserGroupApi",
                    urlHelper.GetUmbracoApiServiceBaseUrl<UserGroupsApiController>(c => c.GetUserGroups())
                },
                {
                    "RedirectsApi", urlHelper.GetUmbracoApiServiceBaseUrl<RedirectsApiController>(c =>
                        c.GetRedirectsForContent(Guid.Empty))
                }
            };


            if (!e.Keys.Contains("Our.Umbraco.RedirectsViewer"))
            {
                e.Add("Our.Umbraco.RedirectsViewer", urlDictionairy);
            }
        }
    }
}
