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
		/// <summary>
		/// Load micro frontend.
		/// </summary>
		/// <param name="services">A <see cref="IServiceCollection"/>.</param>
		/// <param name="assemblyLoadDirectory">The local file path which micro frontend libraries should be loaded from.</param>
		/// <param name="binDirectory">A directory to load any supporting 3rd party libraries from. Leave as null to use a directory named 'bin' with the configured assemblyLoadDirectory.</param>
		/// <returns></returns>
        public static IServiceCollection AddMicrofrontends(this IServiceCollection services, string assemblyLoadDirectory, string binDirectory = null)
        {
            var assembliesToLoad = new List<string>();
			var binAssemblies = new List<string>();

			if (Directory.Exists(
				Path.Combine(
					assemblyLoadDirectory,
					"bin")))
			{
				foreach (var file in Directory.GetFiles(
					Path.Combine(
						assemblyLoadDirectory,
						"bin"),
					"*.dll"))
				{
					binAssemblies.Add(file);
				}
			}

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

			var successfulLoads = new List<string>();

		    while (successfulLoads.Count != binAssemblies.Count)
		    {
			    foreach (var file in binAssemblies)
			    {
				    if (successfulLoads.Contains(file))
				    {
					    continue;
				    }

				    try
				    {
					    AssemblyLoadContext.Default.LoadFromAssemblyPath(file);

					    successfulLoads.Add(file);
				    }
				    catch (ReflectionTypeLoadException)
				    {
				    }
			    }
		    }

			successfulLoads = new List<string>();

		    foreach (var assemblyPath in assembliesToLoad)
		    {
			    if (successfulLoads.Contains(assemblyPath) == false)
			    {
				    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

				    foreach (var type in assembly.GetTypes())
				    {
					    if (typeof(INavElement).IsAssignableFrom(type))
					    {
						    Console.WriteLine("Loading nav element");

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
						    Console.WriteLine("Loading service injector");

						    var loadedType = Activator.CreateInstance(type) as IServiceInjector;

						    services = loadedType.InjectServices(services);
					    }

					    if (typeof(FrontEndComponent).IsAssignableFrom(type))
					    {
						    Console.WriteLine("Loading component");

						    var loadedType = Activator.CreateInstance(type) as ComponentBase;

						    DynamicComponents.Components.Add(
							    loadedType.GetType().Name,
							    loadedType);
					    }
				    }

				    RuntimeAssemblies.Assemblies[fileCounter] = assembly;

				    mvcBuilder.AddApplicationPart(assembly);

				    successfulLoads.Add(assemblyPath);

				    fileCounter++;
			    }
		    }

		    return services;
        }
    }
}
