using System.Collections.Generic;

using Microsoft.AspNetCore.Components;

namespace MicroBlazor.Server
{
    public static class DynamicComponents
    {
	    public static Dictionary<string, ComponentBase> Components = new Dictionary<string, ComponentBase>();
    }
}
