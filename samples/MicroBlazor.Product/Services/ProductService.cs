using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroBlazor.Product.Services
{
    public class ProductService : IProductService
    {
	    /// <inheritdoc />
	    public async Task<List<Models.Product>> GetProductWithVariants()
	    {
		    var products = new List<Models.Product>(3)
		    {
			    new Models.Product()
			    {
				    Name = "Tractor Porsche-Diesel Master 419",
				    ImagePath = "https://micro-frontends.org/0-model-store/images/tractor-red.jpg",
				    ThumbPath = "https://micro-frontends.org/0-model-store/images/tractor-red-thumb.jpg",
				    Price = 66.00M
			    },
			    new Models.Product()
			    {
				    Name = "Tractor Fendt F20 Dieselroß",
				    ImagePath = "https://micro-frontends.org/0-model-store/images/tractor-green.jpg",
				    ThumbPath = "https://micro-frontends.org/0-model-store/images/tractor-green-thumb.jpg",
				    Price = 54.00M
			    },
			    new Models.Product()
			    {
				    Name = "Tractor Eicher Diesel 215/16",
				    ImagePath = "https://micro-frontends.org/0-model-store/images/tractor-blue.jpg",
				    ThumbPath = "https://micro-frontends.org/0-model-store/images/tractor-blue-thumb.jpg",
				    Price = 58.00M
			    }
		    };

		    return products;
	    }
    }
}
