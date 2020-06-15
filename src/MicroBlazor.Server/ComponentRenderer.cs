using Microsoft.AspNetCore.Components;

namespace MicroBlazor.Server
{
    public static class ComponentRenderer
    {
	    public static RenderFragment Render(
		    string componentName)
	    {
		    RenderFragment renderedData;

		    if (DynamicComponents.Components.ContainsKey(componentName))
		    {
			    renderedData = data =>
			    {
				    var type = DynamicComponents.Components[componentName];

				    data.OpenComponent(
					    1,
					    type.GetType());

				    data.CloseComponent();
			    };
		    }
		    else
		    {
			    renderedData = data =>
				    { };
		    }

		    return renderedData;
	    }
    }
}
