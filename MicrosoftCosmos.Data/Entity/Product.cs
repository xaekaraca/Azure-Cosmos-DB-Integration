using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MicrosoftCosmos.Data.Entity;

public class Product
{
    [JsonProperty("id")]
    public string Id { get; set; } = null!;
    
    public string CategoryId { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public decimal Price { get; set; } 
    
    public bool IsDeleted { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ChangedAt { get; set; }
}