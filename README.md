# A Microfrontend Framework for Blazor

![Nuget](https://img.shields.io/nuget/v/MicroBlazor.Core)
![Nuget](https://img.shields.io/nuget/v/MicroBlazor.Server)

MicroBlazor is a framework to handle the loading and configuring of microfrontends within a Blazor web application.

```
Note: This project is still experimental so it's possible that some components will be removed or refactored.
```

```
Second note: Currently, only Blazor Server is supported.
```

## Prerequisites

Before you continue, please make sure you have the latest version of Visual Studio and .Net Core installed. Visit an official [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) site to learn more.

## Samples

There is a sample application in the samples folder that implements the example microfrontend application from [https://micro-frontends.org](https://micro-frontends.org)

## Quick Start Guides

There are two parts to the MicroBlazor framework.

1. MicroBlazor.Server - This is the main application, and the one that would be deployed and hosted in production
2. MicroBlazor.Core - The core library allows the configuration of the Microfrontends themselves

The below guide is intended to give a brief overview for setting up your first MicroBlazor application.

### App Shell

### 1. NuGet packages

The first step is to install the MicroBlazor framework.

```
Install-Package MicroBlazor.Server
```

### 2. Service Injection

Update the Startup.cs file to include the AddMicrofrontends() method.

``` csharp

public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();
    services.AddServerSideBlazor();
    services.AddMicrofrontends(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "microfrontends"));
}

```

The provided parameter is the local path to the folder in which the libraries will be stored for the various different front ends.

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

The MicroBlazor framework also loads any Components from the microfrontend libraries. Because the components are loaded at runtime, they can't be used directly in a Razor page. A helper method (ComponentRenderer.Render) is provided to help with rendering microfrontend components.

```

<div class="col-3">
    @ComponentRenderer.Render("RelatedProducts")
</div>

```

The above code would attempt to render a component named 'RelatedProducts'.

### Microfrontend

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

Add any pages, components or services that are required for your frontend. The beauty of the MicroBlazor framework is that each microfrontend can be treated as it's own standalone application, and can be executed, debugged and tested without the need for the app shell.

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

As well as exposing components and pages, a microfrontend may also want to export a service that other Microfrontends/pages could use. For that, create a new class that implements the IServiceInjector interface and add any required DI.

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

### 6. Deploy

To deploy a microfrontend, build the DLL and copy the compiled library into the folder configured as the assemblyLoadDirectory in your appshell configuration.