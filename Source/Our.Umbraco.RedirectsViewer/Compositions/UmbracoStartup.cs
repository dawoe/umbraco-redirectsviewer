﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Our.Umbraco.RedirectsViewer.Controllers;
using Our.Umbraco.RedirectsViewer.Mapping;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;

namespace Our.Umbraco.RedirectsViewer.Compositions
{
    public class UmbracoStartup:IUserComposer
    {
        public void Compose(Composition composition)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<UserGroupProfile>();
            });

            IMapper mapper = new Mapper(config);

            composition.Register(mapper);

            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
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