using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCosmos.Data.Entity;

public class Information
{
    [Key]
    [Required]
    public string UserId { get; set; } = null!;
    
    [Required]
    public string CompanyId { get; set; } = null!;
    
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    public string FirstName { get; set; } = null!;
    
    [Required]
    public string LastName { get; set; } = null!;
    
    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime ChangedAt { get; set; }
}