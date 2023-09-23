using MicrosoftCosmos.Data.Entity;
using MicrosoftCosmos.Data.Models;

namespace MicrosoftCosmos.Data.Mapper;

public static class ProductMapper
{
    public static Product ToEntity(ProductCreateModel productCreateModel)
    {
        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            CategoryId = productCreateModel.CategoryId,
            Name = productCreateModel.Name,
            Description = productCreateModel.Description,
            Price = productCreateModel.Price,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public static ProductViewModel ToViewModel(this Product product)
    {
        return new ProductViewModel
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public static List<ProductViewModel> ToViewModelList(this IEnumerable<Product> products)
    {
        return products.Select(ToViewModel).ToList();
    }
    
    public static Product ToEntity(this ProductUpdateModel productUpdateModel, Product product)
    {
        product.Name = productUpdateModel.Name ?? product.Name;
        product.Description = productUpdateModel.Description ?? product.Description;
        product.Price = productUpdateModel.Price ?? product.Price;

        return product;
    }
}