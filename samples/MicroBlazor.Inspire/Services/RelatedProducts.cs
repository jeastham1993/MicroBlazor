using System.Collections.Generic;
using System.Threading.Tasks;

using MicroBlazor.Inspire.Models;

namespace MicroBlazor.Inspire.Services
{
    public class RelatedProducts : IRelatedProducts
    {
	    /// <inheritdoc />
	    public async Task<List<RelatedProduct>> LoadRelatedProducts()
	    {
		    var relatedProducts = new List<RelatedProduct>(3)
		    {
			    new RelatedProduct()
			    {
				    ProductImage = "https://micro-frontends.org/0-model-store/images/reco_3.jpg"
			    },
			    new RelatedProduct()
			    {
				    ProductImage = "https://micro-frontends.org/0-model-store/images/reco_2.jpg"
			    },
			    new RelatedProduct()
			    {
				    ProductImage = "https://micro-frontends.org/0-model-store/images/reco_1.jpg"
			    }
		    };

		    return relatedProducts;
	    }
    }
}
