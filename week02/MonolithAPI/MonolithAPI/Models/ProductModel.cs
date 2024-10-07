using System.ComponentModel.DataAnnotations;

namespace MonolithAPI.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public double Price { get; set; }
    public string? Description { get; set; }
}
