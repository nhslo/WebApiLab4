using System.ComponentModel.DataAnnotations;

namespace WebApiLab4.DTOs;

public class ProductInputDto
{
    [Required, StringLength(160, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "1", "1000000000")]
    public decimal Price { get; set; }

    [Required, StringLength(100, MinimumLength = 2)]
    public string Category { get; set; } = string.Empty;
}
