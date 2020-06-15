using System.Collections.Generic;
using System.Threading.Tasks;

using MicroBlazor.Inspire.Models;

namespace MicroBlazor.Inspire.Services
{
    public interface IRelatedProducts
    {
	    Task<List<RelatedProduct>> LoadRelatedProducts();
    }
}
