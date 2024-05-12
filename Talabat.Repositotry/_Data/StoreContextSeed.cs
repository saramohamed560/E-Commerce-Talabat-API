using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Repositotry.Data
{
    public  static  class StoreContextSeed
    {
        public async static Task SeedAsync(StoreContext dbContext)
        {
            if (dbContext.ProductBrands.Count() == 0)
            {      //Read File as string (
                var brandsData = File.ReadAllText("../Talabat.Repositotry/_Data/DataSeed/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                if (brands?.Count() > 0) {
                    foreach (var brand in brands)
                    {
                       await  dbContext.Set<ProductBrand>().AddAsync(brand);
                    }
                }
            }

            if(dbContext.Set<ProductCategory>().Count() == 0)
            {
                var CategoryData = File.ReadAllText("../Talabat.Repositotry/_Data/DataSeed/categories.json");
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(CategoryData);
                if (categories?.Count() > 0)
                {
                    foreach (var category in categories)
                    {
                        await dbContext.Set<ProductCategory>().AddAsync(category);
                    }
                }
            }

            if (dbContext.Set<Product>().Count() == 0)
            {
                var productData = File.ReadAllText("../Talabat.Repositotry/_Data/DataSeed/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productData);
                if (products?.Count() > 0)
                {
                    foreach (var product in products)
                    {
                       await dbContext.Set<Product>().AddAsync(product);
                    }
                }
            }

			if (dbContext.Set<DeliveryMethod>().Count() == 0)
			{      //Read File as string (
				var DeliveryMethodsData = File.ReadAllText("../Talabat.Repositotry/_Data/DataSeed/delivery.json");
				var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
				if (DeliveryMethods?.Count > 0)
				{
					foreach (var deivery in DeliveryMethods)
					{
						await dbContext.Set<DeliveryMethod>().AddAsync(deivery);
					}
					await dbContext.SaveChangesAsync();

				}
			}

            await dbContext.SaveChangesAsync();


		}
	}
}
