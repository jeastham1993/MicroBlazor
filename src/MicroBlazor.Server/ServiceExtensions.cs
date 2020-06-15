using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

using MicroBlazor.Core;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace MicroBlazor.Server
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMicrofrontends(this IServiceCollection services, string assemblyLoadDirectory)
        {
            var assembliesToLoad = new List<string>();

		    if (Directory.Exists(assemblyLoadDirectory))
		    {
			    foreach (var file in Directory.GetFiles(
				    assemblyLoadDirectory,
				    "*.dll"))
			    {
				    assembliesToLoad.Add(file);
			    }
		    }

		    var mvcBuilder = services.AddRazorPages()
			    .AddRazorRuntimeCompilation(
			    option =>
			    {
				    option.FileProviders.Add(new PhysicalFileProvider(assemblyLoadDirectory));

				    foreach (var assembly in assembliesToLoad)
				    {
					    option.AdditionalReferencePaths.Add(assembly);
				    }
			    });

			RuntimeAssemblies.Assemblies = new Assembly[assembliesToLoad.Count];

			var fileCounter = 0;

		    foreach (var file in assembliesToLoad)
		    {
			    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);

			    foreach (var type in assembly.GetTypes())
			    {
				    if (typeof(INavElement).IsAssignableFrom(type))
				    {
					    var loadedType = Activator.CreateInstance(type) as INavElement;

					    if (loadedType != null
					        && string.IsNullOrEmpty(loadedType.NavText) == false
					        && string.IsNullOrEmpty(loadedType.NavLink) == false)
					    {
						    NavElements.LoadedNavElements.Add(loadedType);
					    }
				    }

				    if (typeof(IServiceInjector).IsAssignableFrom(type))
				    {
					    var loadedType = Activator.CreateInstance(type) as IServiceInjector;

					    services = loadedType.InjectServices(services);
				    }

				    if (typeof(ComponentBase).IsAssignableFrom(type))
				    {
					    var loadedType = Activator.CreateInstance(type) as ComponentBase;

					    DynamicComponents.Components.Add(
						    loadedType.GetType().Name,
						    loadedType);
				    }
			    }

			    RuntimeAssemblies.Assemblies[fileCounter] = assembly;

			    mvcBuilder.AddApplicationPart(assembly);

			    fileCounter++;
		    }

		    return services;
        }
    }
}
