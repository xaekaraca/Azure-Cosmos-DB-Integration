using CosmosDbEntegrasyon.Api.Services;
using EntityFrameworkCosmos.Data.Mappers;
using EntityFrameworkCosmos.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDbEntegrasyon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InformationController : ControllerBase
{
    private readonly InformationService _informationService;

    public InformationController(InformationService informationService)
    {
        _informationService = informationService;
    }
    //Normally we do not get CompanyId information from controller.
    //We get it from either JWT or Cookie or Session.
    //But for this example, we get it "[FromQuery]".
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] InformationQueryFilterModel queryFilterModel,CancellationToken cancellationToken = default)
    {
        var response = await _informationService.GetAsync(queryFilterModel,cancellationToken);
        
        return Ok(InformationMapper.ToViewModelList(response));
    }
    
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetAsync([FromRoute] string userId,[FromQuery] string companyId,CancellationToken cancellationToken = default)
    {
        var response = await _informationService.GetAsync(userId,companyId);

        if (response == null)
            return NoContent();
        
        return Ok(InformationMapper.ToViewModel(response));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] InformationCreateModel createModel,CancellationToken cancellationToken = default)
    {
        var response = await _informationService.CreateAsync(createModel,cancellationToken);
        
        return Ok(InformationMapper.ToViewModel(response));
    }
    
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] string userId,[FromQuery] string companyId,[FromBody] InformationUpdateModel updateModel,CancellationToken cancellationToken = default)
    {
        var entity = await _informationService.GetAsync(userId,companyId);
        
        if (entity == null)
            return NoContent();
        
        var response = await _informationService.UpdateAsync(InformationMapper.ToEntity(updateModel,entity),cancellationToken);
        
        return Ok(InformationMapper.ToViewModel(response));
    }
    
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string userId,[FromQuery] string companyId,CancellationToken cancellationToken = default)
    {
        await _informationService.DeleteAsync(userId,companyId,cancellationToken);
        
        return Ok();
    }
}