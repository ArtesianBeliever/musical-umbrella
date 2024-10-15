using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

public class ProductController : Controller
{
    private ApplicationDbContext _dbContext;
    
    private Repository<Product> products;
    private Repository<Ingredient> ingredients;
    private Repository<Category> categories;
    
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _dbContext = context;
        products = new Repository<Product>(context);
        ingredients = new Repository<Ingredient>(context);
        categories = new Repository<Category>(context);
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        return View(await products.GetAllAsync());
    }

    [HttpGet]
    public async Task<IActionResult> AddEdit(int id)
    {
        ViewBag.Ingredients = await ingredients.GetAllAsync();
        ViewBag.categories = await categories.GetAllAsync();    
        if (id == 0)
        {
            ViewBag.Operation = "Add";
            return View(new Product());
        }
        else
        {
            Product product = await _dbContext.Products
                .Include(x => x.ProductCategories).ThenInclude(x => x.Category)
                .Include(x => x.ProductIngredients).ThenInclude(x => x.Ingredient)
                .FirstOrDefaultAsync(x => x.ProductId == id);
            ViewBag.Operation = "Edit";
            return View(product);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddEdit(Product product, int[] ingredientIds, int[] categoryIds)
    {
        ViewBag.Ingredients = await ingredients.GetAllAsync();
        ViewBag.categories = await categories.GetAllAsync();
        if (ModelState.IsValid)
        {
            // if (product.ImageFile != null)
            // {
            //     string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            //     string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
            //     string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            //     using (var fileStream = new FileStream(filePath, FileMode.Create))
            //     {
            //         await product.ImageFile.CopyToAsync(fileStream);
            //     }
            //     product.ImageUrl = uniqueFileName;
            // }

            if (product.ProductId == 0)
            {
                if (product.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = uniqueFileName;
                }
                ViewBag.Ingredients = await ingredients.GetAllAsync();
                ViewBag.categories = await categories.GetAllAsync();
                foreach (int id in categoryIds)
                {
                    product.ProductCategories?.Add(new ProductCategory() { CategoryId = id, ProductId = product.ProductId });
                }
                foreach (int id in ingredientIds)
                {
                    product.ProductIngredients?.Add(new ProductIngredient { IngredientId = id, ProductId = product.ProductId });
                }
                await products.UpdateAsync(product);
                return RedirectToAction("Index", "Product");
            }
            else
            {
                var existingProduct =  await _dbContext.Products
                    .Include(x => x.ProductCategories).ThenInclude(x => x.Category)
                    .Include(x => x.ProductIngredients).ThenInclude(x => x.Ingredient)
                    .FirstOrDefaultAsync(x => x.ProductId == product.ProductId);
                
                if (existingProduct == null)
                {
                    ModelState.AddModelError("ProductId", "Product not found");
                    ViewBag.Ingredients = await ingredients.GetAllAsync();
                    ViewBag.categories = await categories.GetAllAsync();
                    return View(product);
                }
                
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.Stock = product.Stock;
                existingProduct.ProductCategories.Clear();
                foreach (int id in categoryIds)
                {
                    existingProduct.ProductCategories?.Add(new ProductCategory() { CategoryId = id, ProductId = product.ProductId });
                }
                
                existingProduct.ProductIngredients.Clear();
                foreach (int id in ingredientIds)
                {
                    existingProduct.ProductIngredients?.Add(new ProductIngredient { IngredientId = id, ProductId = product.ProductId });
                }
                
                var oldImageUrl = Path.Combine(_webHostEnvironment.WebRootPath,"images",existingProduct.ImageUrl);
                bool imageUpdated = false;
                if (product.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    existingProduct.ImageUrl = uniqueFileName;
                    imageUpdated = true;
                }

                try
                {
                    await products.UpdateAsync(existingProduct);
                    if (imageUpdated)
                        System.IO.File.Delete(Path.Combine(oldImageUrl));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ProductId", ex.GetBaseException().Message);
                    ViewBag.Ingredients = await ingredients.GetAllAsync();
                    ViewBag.categories = await categories.GetAllAsync();
                    return View(product);
                }
                
                return RedirectToAction("Index", "Product");
            }
        }
        return RedirectToAction("Index", "Product");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await products.DeleteAsync(id);
            return RedirectToAction("Index", "Product");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("","Product Not Found");
            return RedirectToAction("Index", "Product");
        }
    }
}