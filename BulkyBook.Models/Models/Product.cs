using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    public string Title { get; set; }
    [Required]
    public string ISBN { get; set; }
    [Required]
    public string Author { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    [DisplayName("List Price")]
    [Range(1,1000)]
    public double ListPrice { get; set; }
    
    [Required]
    [DisplayName("Price for 50+")]
    [Range(1,1000)]
    public double ListPrice50 { get; set; }
    
    [Required]
    [DisplayName("Price for 100+")]
    [Range(1,1000)]
    public double ListPrice100 { get; set; }
    
    
    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    [ValidateNever]
    public Category Category { get; set; }
    
    [ValidateNever]
    public string ImageUrl { get; set; }

    
}