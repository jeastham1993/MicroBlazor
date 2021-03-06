# A micro frontend Framework for Blazor

![Nuget](https://img.shields.io/nuget/v/MicroBlazor.Core)
![Nuget](https://img.shields.io/nuget/v/MicroBlazor.Server)

MicroBlazor is a framework to handle the loading and configuring of micro frontends within a Blazor web application.

```
Note: This project is still experimental so it's possible that some components will be removed or refactored.
```

```
Second note: Currently, only Blazor Server is supported.
```

## Prerequisites

Before you continue, please make sure you have the latest version of Visual Studio and .Net Core installed. Visit an official [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) site to learn more.

## Samples

There is a sample application in the samples folder that implements the example micro frontend application from [https://micro-frontends.org](https://micro-frontends.org)

## Quick Start Guides

There are two parts to the MicroBlazor framework.

1. MicroBlazor.Server - This is the main application, and the one that would be deployed and hosted in production
2. MicroBlazor.Core - The core library allows the configuration of the micro frontends themselves

The below guide is intended to give a brief overview for setting up your first MicroBlazor application.

### App Shell

### 1. NuGet packages

The first step is to install the MicroBlazor framework.

```
Install-Package MicroBlazor.Server
```

### 2. Service Injection

Update the Startup.cs file to include the Addmicro frontends() method.

``` csharp

public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();
    services.AddServerSideBlazor();
    services.Addmicro frontends(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "micro frontends"), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "micro frontends", "bin"));
}

```

There are two parameters provided to the Addmicro frontends method:

1. assemblyLoadDirectory - The local directory from which micro frontend libraries should be loaded
2. binDirectory - The local directory in which 3rd part dependencies should be loaded from. This should be a different path to the assemblyLoadDirectory. Defaults to 'bin' within the assemblyLoadDirectory.

### 3. Add Routing

The router in the App.razor file then needs to be updated to be aware of the assemblies loaded at runtime, the MicroBlazor.Server package exposes an easy to use static class for settings this up, RuntimeAssemblies.Assemblies.

``` csharp

@using MicroBlazor.Server

<Router AppAssembly="@typeof(Program).Assembly" AdditionalAssemblies="RuntimeAssemblies.Assemblies">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p>Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>

```

### 4. Navigation

The MicroBlazor framework initializes a static class with all of the relevant nav links, to access any loaded navigation elements, use the NavElements.LoadedNavElements property.

``` csharp

@foreach (var navElement in NavElements.LoadedNavElements)
{
    <li class="nav-item px-3">
        <NavLink class="nav-link" href="@navElement.NavLink">
            @navElement.NavText
        </NavLink>
    </li>
}

```

### 4. Using Components

The MicroBlazor framework also loads any Components from the micro frontend libraries. Because the components are loaded at runtime, they can't be used directly in a Razor page. A helper method (ComponentRenderer.Render) is provided to help with rendering micro frontend components.

```

<div class="col-3">
    @ComponentRenderer.Render("RelatedProducts")
</div>

```

The above code would attempt to render a component named 'RelatedProducts'.

### micro frontend

Creating a micro frontend is exactly like creating a new Blazor application.

### 1. Start a new project

``` bash

dotnet new blazorserver

```

### 2. Nuget Packages

Once the new Blazor server app has been created, add the MicroBlazor.Core library

```
Install-Package MicroBlazor.Core
```

### 3. Create your app

Add any pages, components or services that are required for your frontend. The beauty of the MicroBlazor framework is that each micro frontend can be treated as it's own standalone application, and can be executed, debugged and tested without the need for the app shell.

### 4. Adding Navigation Elements

To expose a navigation element to the AppShell, create a new class for each NavLink you would like to create. The class should inherit from the INavElement interface and be fully implemented.

``` csharp

public class SamplePageNav : INavElement
{
    /// <inheritdoc />
    public string NavText => "Sample Page";

    /// <inheritdoc />
    public string NavLink => "sample";
}

```

### 5. Adding dependency injection

As well as exposing components and pages, a micro frontend may also want to export a service that other micro frontends/pages could use. For that, create a new class that implements the IServiceInjector interface and add any required DI.

``` csharp

public class ServiceInjector : IServiceInjector
{
    /// <inheritdoc />
    public IServiceCollection InjectServices(
        IServiceCollection services)
    {
        services.AddTransient<IRelatedProducts, RelatedProducts>();

        return services;
    }
}

```

### 6. Adding custom components

To add a custom component, is must inherit from the FrontEndComponent class.

``` csharp

@using MicroBlazor.Inspire.Models
@using MicroBlazor.Inspire.Services

@inherits MicroBlazor.Core.FrontEndComponent

<h3>Related Products</h3>
@foreach (var product in Products)
{
    <div class="row">
        <div class="col-12">
            <img style="width: 70%; margin-left: auto; margin-right: auto; display: block;" src="@product.ProductImage"/>
        </div>
    </div>
}

@code {

    [Inject]
    public IRelatedProducts RelatedProductService { get; set; }

    protected List<RelatedProduct> Products { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        this.Products = await this.RelatedProductService.LoadRelatedProducts().ConfigureAwait(
            false);
    }

}

```

### 6. Deploy

To deploy a micro frontend, build the DLL and copy the compiled library into the folder configured as the assemblyLoadDirectory in your appshell configuration as well as copying any 3rd party dependencies into the configured bin folder.