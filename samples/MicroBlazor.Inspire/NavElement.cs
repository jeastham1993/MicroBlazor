using MicroBlazor.Core;

namespace MicroBlazor.Inspire
{
    public class NavElement : INavElement
    {
	    /// <inheritdoc />
	    public string NavText => "Customers";

	    /// <inheritdoc />
	    public string NavLink => "/customers";
    }
}
