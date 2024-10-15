using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication2.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    [ValidateNever]
    public ICollection<ProductCategory> ProductCategories { get; set; }
}