using System.ComponentModel.DataAnnotations;
using CosmosDbEntegrasyon.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MicrosoftCosmos.Data.Mapper;
using MicrosoftCosmos.Data.Models;

namespace CosmosDbEntegrasyon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }
    
    [HttpGet("{categoryId}/{id}")]
    public async Task<IActionResult> GetAsync(string id, string categoryId)
    {
        var product = await _productService.GetAsync(id, categoryId);

        return Ok(product.ToViewModel());
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] ProductQueryFilterModel queryFilterModel, CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetAllWithLinqAsync(queryFilterModel, cancellationToken);

        return Ok(products.ToViewModelList());
    }
    
    [HttpGet("iterator")]
    public async Task<IActionResult> GetAllWithIteratorAsync([FromQuery][Required] string categoryId, CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetAllWithIteratorAsync(categoryId, cancellationToken);

        return Ok(products.ToViewModelList());
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(ProductCreateModel productCreateModel, CancellationToken cancellationToken = default)
    {
        var product = await _productService.CreateAsync(productCreateModel, cancellationToken);

        return Ok(product.ToViewModel());
    }
    
    [HttpPut("{categoryId}/{id}")]
    public async Task<IActionResult> UpdateAsync(string id, string categoryId,ProductUpdateModel productUpdateModel, CancellationToken cancellationToken = default)
    {
        var product = await _productService.UpdateAsync(id, categoryId, productUpdateModel,cancellationToken);

        return Ok(product.ToViewModel());
    }
    
    [HttpDelete("{categoryId}/{id}")]
    public async Task<IActionResult> DeleteAsync(string id, string categoryId, CancellationToken cancellationToken = default)
    {
        await _productService.DeleteAsync(id, categoryId, cancellationToken);

        return Ok();
    }
}