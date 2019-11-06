# Umbraco redirects viewer

[![Build status](https://ci.appveyor.com/api/projects/status/vqk2mxw245qxnnf8?svg=true)](https://ci.appveyor.com/project/dawoe/umbraco-redirectsviewer)

|NuGet Packages    |Version           |
|:-----------------|:-----------------|
|**Release**|[![NuGet download](http://img.shields.io/nuget/v/Our.Umbraco.RedirectsViewer.svg)](https://www.nuget.org/packages/Our.Umbraco.RedirectsViewer/)
|**Pre-release**|[![MyGet Pre Release](https://img.shields.io/myget/dawoe-umbraco/vpre/Our.Umbraco.RedirectsViewer.svg)](https://www.myget.org/feed/dawoe-umbraco/package/nuget/Our.Umbraco.RedirectsViewer)

|Umbraco Packages  |                  |
|:-----------------|:-----------------|
|**Release**|[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/redirects-viewer/) 
|**Pre-release**| [![AppVeyor Artifacts](https://img.shields.io/badge/appveyor-umbraco-orange.svg)](https://ci.appveyor.com/project/dawoe/umbraco-redirectsviewer/build/artifacts)


This package will install a property editor that will allow you to view and manage redirects for the current page.

Deleting and creating is by default disabled (except for admins), but can be enabled in the datatype settings. It will allow you to select which user groups have the rights to do these actions.



## How to use ##

1. In the Developer section, create a new Data Type, give it an appropriate name, e.g. "Redirects Viewer", choose the property editor with the same name, and set the permissions to your liking (admins have full access).

2. Go to the Settings section. On the doc types for the pages which you want to redirect **to**, add a new property, give it an appropriate name, e.g. "Allow Redirects From", and choose the data type you created above. A composite doc type which you're including on many/all pages might also be a good candidate for where to place the new property.

3. Go to the Contents section and select a page which you want to be the target of a redirect, find the property you created, click "Add redirect", and enter the path you are redirecting **from**. The path should not contain the domain name, and may/may not start with a slash (it doesn't matter).

4. That's it. Except one gotcha! If you have a site with multiple roots, e.g. one per language/country, and you have not specifically assigned domain names to these roots, then the redirects will not work out of the box. The work around is to add a setting to your web.config: under the &lt;appsettings> element add the following: &lt;add key="Our.Umbraco.RedirectsViewer:IgnoreWildCardDomains" value="true" />, and reload the website.



## Support this package ##

If you like this package and use it in your website, consider becoming a patreon to support ongoing maintenance

[https://www.patreon.com/dawoe](https://t.co/TBsvTMnOLB)
