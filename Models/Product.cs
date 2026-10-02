using System.ComponentModel.DataAnnotations;

namespace WebApiLab4.Models;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "1", "1000000000")]
    public decimal Price { get; set; }

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;
}
