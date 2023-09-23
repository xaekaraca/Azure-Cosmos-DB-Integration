using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCosmos.Data.Entity;

public class Information
{
    [Key]
    public string UserId { get; set; } = null!;
    
    public string CompanyId { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime ChangedAt { get; set; }
}