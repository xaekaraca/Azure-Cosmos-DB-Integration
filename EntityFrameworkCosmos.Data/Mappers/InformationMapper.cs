using EntityFrameworkCosmos.Data.Entity;
using EntityFrameworkCosmos.Data.Models;

namespace EntityFrameworkCosmos.Data.Mappers;

public class InformationMapper
{
    public static Information ToEntity(InformationCreateModel model)
    {
        return new Information
        {
            UserId = Guid.NewGuid().ToString(),
            CompanyId = model.CompanyId,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };
    }
    
    public static Information ToEntity(InformationUpdateModel model,Information entity)
    {
        entity.Email = model.Email ?? entity.Email;
        entity.FirstName = model.FirstName ?? entity.FirstName;
        entity.LastName = model.LastName ?? entity.LastName;
        return entity;
    }
    
    public static InformationViewModel ToViewModel(Information entity)
    {
        return new InformationViewModel
        {
            UserId = entity.UserId,
            CompanyId = entity.CompanyId,
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            CreatedAt = entity.CreatedAt,
            ChangedAt = entity.ChangedAt,
        };
    }
    
    public static IEnumerable<InformationViewModel> ToViewModelList(IEnumerable<Information> entities)
    {
        return entities.Select(ToViewModel);
    }
}