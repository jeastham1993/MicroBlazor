using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroBlazor.Product.Services
{
    public interface IProductService
    {
	    Task<List<Models.Product>> GetProductWithVariants();
    }
}
