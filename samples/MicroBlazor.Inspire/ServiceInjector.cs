using MicroBlazor.Core;
using MicroBlazor.Inspire.Services;

using Microsoft.Extensions.DependencyInjection;

namespace MicroBlazor.Inspire
{
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
}
