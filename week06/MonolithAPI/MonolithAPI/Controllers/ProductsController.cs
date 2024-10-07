using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonolithAPI.DTOs.Reponse;
using MonolithAPI.DTOs.Request;
using MonolithAPI.Models;

namespace MonolithAPI.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public ProductsController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<ProductDTO>))]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _appDbContext.Products.Select(x => new ProductDTO
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price
        }).ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(ProductDetailDTO))]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var curProduct = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (curProduct == null)
        {
            return NotFound();
        }
        var result = new ProductDetailDTO
        {
            Id = curProduct.Id,
            Name = curProduct.Name,
            Price = curProduct.Price,
            Description = curProduct.Description,
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(statusCode: StatusCodes.Status201Created, type: typeof(ProductDetailDTO))]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostProduct(CreateProductDTO request)
    {
        var newProduct = new ProductModel
        {
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedTime = DateTime.UtcNow
        };
        _appDbContext.Products.Add(newProduct);
        await _appDbContext.SaveChangesAsync();
        var result = new ProductDetailDTO
        {
            Id = newProduct.Id,
            Name = newProduct.Name,
            Price = newProduct.Price,
            Description = newProduct.Description,
        };
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutProduct(Guid id, UpdateProductDTO request)
    {
        var curProduct = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id && x.CreatedBy == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (curProduct == null || curProduct.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return NotFound();
        }

        curProduct.Name = request.Name;
        curProduct.Price = request.Price;
        curProduct.Description = request.Description;
        curProduct.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
        curProduct.UpdatedTime = DateTime.UtcNow;

        _appDbContext.Products.Update(curProduct);
        await _appDbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var curProduct = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id && x.CreatedBy == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (curProduct == null)
        {
            return NotFound();
        }
        _appDbContext.Products.Remove(curProduct);
        await _appDbContext.SaveChangesAsync();
        return NoContent();
    }
}
