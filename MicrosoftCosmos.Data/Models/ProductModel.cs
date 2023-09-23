namespace MicrosoftCosmos.Data.Models;

public class ProductViewModel
{
    public string Id { get; set; } = null!;
    
    public string CategoryId { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public decimal Price { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

public class ProductCreateModel
{
    public string CategoryId { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public decimal Price { get; set; }
}

public class ProductUpdateModel
{
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public decimal? Price { get; set; }
}

public class ProductQueryFilterModel
{
    public string? CategoryId { get; set; }
    
    public decimal? MinPrice { get; set; }
    
    public decimal? MaxPrice { get; set; }
    
    public int? PageSize { get; set; }
    
    public int? PageNumber { get; set; }
}