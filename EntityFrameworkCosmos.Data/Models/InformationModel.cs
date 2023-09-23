using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCosmos.Data.Models;

public class InformationViewModel
{
    public string? UserId { get; set; }
    
    public string? CompanyId { get; set; }
    
    public string? Email { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime ChangedAt { get; set; }
}
public class InformationCreateModel
{
    [Required]
    public string CompanyId { get; set; } = null!;
    
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    public string FirstName { get; set; } = null!;
    
    [Required]
    public string LastName { get; set; } = null!;
}
public class InformationUpdateModel
{
    public string? Email { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
}
public class InformationQueryFilterModel
{
    public string CompanyId { get; set; } = null!;
    
    public int? PageSize { get; set; }
    
    public int? PageNumber { get; set; }
}