using Microsoft.Extensions.DependencyInjection;

namespace MicroBlazor.Core
{
    public interface IServiceInjector
    {
	    IServiceCollection InjectServices(
		    IServiceCollection services);
    }
}
