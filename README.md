# Sitecore Multisite Http Module

A Sitecore module to allow for easy configuration of site specific 404 pages, 500 pages & robots.txt files.


## Installation Instructions

### Nuget
This repository contains a nuspec file. If you wish you can host the project on your internal nuget repo and from there  install using the nuget package manager within Visual Studio. If installing using this method, you still need to manually edit your Global.ascx.cs to include the reference to the error handling code

```c#
public void Application_Error(object sender, EventArgs args)
{
    Sitecore.MultisiteHttpModule.Errors.ErrorHandler.HandleError(Server.GetLastError());
}
```

### Manual
* Build project and reference DLL in your project
* Include the httpRequestBegin pipeline processor if you want to make use of the 404 functionality

```xml
<httpRequestBegin>
    <processor type="Sitecore.MultisiteHttpModule.NotFound.NotFoundHandler, Sitecore.MultisiteHttpModule" patch:after="processor[@type='Sitecore.Pipelines.HttpRequest.ItemResolver, Sitecore.Kernel']" />
</httpRequestBegin>

```

* Insert web.config sections

```xml
<configSections>
    <section name="multisiteHttpModule" type="Sitecore.MultisiteHttpModule.Configuration.MultisiteHttpModuleSettings, Sitecore.MultisiteHttpModule" />
</configSections>

<multisiteHttpModule defaultErrorPage="/error.html" errorsEnabled="True" notFoundEnabled="True">
    <exclude404Rules>
        <add type="Contains" match="~/media/" />
        <add type="Contains" match="~/icon/" />
        <add type="Contains" match="~/link.aspx" />
        <add type="StartsWith" match="/sitecore" />
        <add type="Contains" match=".asmx" />
        <add type="Contains" match=".ashx" />
    </exclude404Rules>
</multisiteHttpModule>

<system.webServer>
    <handlers>
        <add name="RobotsHandler" path="/robots.txt" verb="GET" type="Sitecore.MultisiteHttpModule.Robots.RobotsHandler, Sitecore.MultisiteHttpModule" />
    </handlers>
</system.webServer>
```
* Add error handling code into Global.ascx.cs

```c#
public void Application_Error(object sender, EventArgs args)
{
    Sitecore.MultisiteHttpModule.Errors.ErrorHandler.HandleError(Server.GetLastError());
}
```

## Usage

After installing the module, you need to include the new attributes on your existing Sitecore sites node. If you dont want to add one of the functions to a site, then dont include that attribute and the code will fallback to the default Sitecore functionality.

```xml
<site name="MySite" 
      ...
      notFoundPageId="{BFA65433-4552-4507-9375-C96137287640}"
      errorPagePath="/errors/MySite.html" 
      robotsTxtLocation="/robots/MySite.robots.txt" />
```

### 404 Page usage
The 404 page functionality can be configured by populating the **notFoundPageId** attribute on the site node. This needs to be populated with the ID of the item you wish to use for the 404 page. If the item isn't found then it will fallback to the default Sitecore 404 functionality.

#### 404 Configuration
If you want to exclude certain URL's from being processed by the 404 handler then you can use the **exclude404Rules** configuration section in the web.config, this will allow you to exclude URL's from being processed by checking for a string value in the URL. You can check whether a URL **Contains**, **StartsWith** or **EndsWith** the value. The default ones are as follows:

```xml
<add type="Contains" match="~/media/" />
<add type="Contains" match="~/icon/" />
<add type="Contains" match="~/link.aspx" />
<add type="StartsWith" match="/sitecore" />
<add type="Contains" match=".asmx" />
<add type="Contains" match=".ashx" />
```

#### Points to note:
This will not redirect to your 404 page as that results in a 302 response being returned to the browser (a known issue with Sitecore) which is incorrect. Instead it will simply render the 404 content on the requested URL. You then need set the response code to be 404 on that layout using the following code

```c#
HttpContext.Current.Response.StatusCode = 404;
```

### 500 Page usage
The 500 page functionality can be configured by populating the **errorPagePath** attribute on the site node. This needs to be populated with the url to forward the user to when an unhandled error is encountered. It is recommended to use a static page as the target and not a Sitecore URL, as this will then still function if major issues (e.g. database connectivity) occur.

### Robots.txt usage
The robots.txt functionality can be configured by populating the **robotsTxtLocation** attribute on the site node. This needs to be populated with the location of a text document containing the contents you wish to display when robots.txt is called for that site. If this is specified then the standard robots.txt will be returned, if it exists.

## Credits

This module was built upon the work in two blog posts by:
[Anders Laub](http://laubplusco.net/handling-404-sitecore-avoid-302-redirects/ "Handling 404 in a Sitecore multisite solution and avoid 302 redirects.") &amp;
[Brian Pederson](http://briancaos.wordpress.com/2013/03/21/sitecore-404-without-302/ "Sitecore 404 without 302")