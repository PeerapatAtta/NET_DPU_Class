using System.ComponentModel.DataAnnotations;

namespace MonolithAPI.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public double Price { get; set; }
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedTime { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedTime { get; set; }
}
