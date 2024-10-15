
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class ApplicationDbContext : IdentityDbContext <ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products {get;set;}
        public DbSet<Category> Categories {get;set;}
        public DbSet<Order> Orders {get;set;}
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Ingredient> Ingredients {get; set;}
        public DbSet<ProductIngredient> ProductIngredients{get; set;}
        public DbSet<ProductCategory> ProductCategories {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Composite key Products to Ingredients
            builder.Entity<ProductIngredient>()
                .HasKey(pi => new { pi.ProductId, pi.IngredientId });
            
            builder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Ingredient)
                .WithMany(i => i.ProductIngredients)
                .HasForeignKey(pi => pi.IngredientId);
            
            builder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId);
            // Composite Key Products to Categories
            builder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });
            
            builder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);
            
            builder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);
            //Seed Data
            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Appetizer" },
                new Category { CategoryId = 2, Name = "Entree" },
                new Category { CategoryId = 3, Name = "Side Dish" },
                new Category { CategoryId = 4, Name = "Dessert" },
                new Category { CategoryId = 5, Name = "Beverage" }
            );
            builder.Entity<Ingredient>().HasData(
                new Ingredient() { IngredientId = 1, Name = "Beef" },
                new Ingredient() { IngredientId = 2, Name = "Chicken" },
                new Ingredient() { IngredientId = 3, Name = "Fish" },
                new Ingredient() { IngredientId = 4, Name = "Tortilla" },
                new Ingredient() { IngredientId = 5, Name = "Lettuce" },
                new Ingredient() { IngredientId = 6, Name = "Tomato" }
            );
            builder.Entity<Product>().HasData(
                new Product()
                {
                    ProductId = 1,
                    Name = "Beef Taco",
                    Description = "Beef Taco",
                    Price = 2.50m,
                    Stock = 100
                },

                new Product()
                {
                    ProductId = 2,
                    Name = "Chicken Taco",
                    Description = "Chicken Taco",
                    Price = 1.90m,
                    Stock = 102
                },
                new Product()
                {
                    ProductId = 3,
                    Name = "Fish Taco",
                    Description = "Fish Taco",
                    Price = 2.50m,
                    Stock = 90
                }
            );
            builder.Entity<ProductIngredient>().HasData(
                new ProductIngredient() {ProductId = 1, IngredientId = 1},
                new ProductIngredient() {ProductId = 1, IngredientId = 4},
                new ProductIngredient() {ProductId = 1, IngredientId = 5},
                new ProductIngredient() {ProductId = 1, IngredientId = 6},
                
                new ProductIngredient() {ProductId = 2, IngredientId = 2},
                new ProductIngredient() {ProductId = 2, IngredientId = 4},
                new ProductIngredient() {ProductId = 2, IngredientId = 5},
                new ProductIngredient() {ProductId = 2, IngredientId = 6},
                
                new ProductIngredient() {ProductId = 3, IngredientId = 3},
                new ProductIngredient() {ProductId = 3, IngredientId = 4},
                new ProductIngredient() {ProductId = 3, IngredientId = 5},
                new ProductIngredient() {ProductId = 3, IngredientId = 6}
            );
            builder.Entity<ProductCategory>().HasData(
                new ProductCategory() { ProductId = 1, CategoryId = 2 },
                new ProductCategory() { ProductId = 2, CategoryId = 2 },
                new ProductCategory() { ProductId = 3, CategoryId = 2 }
            );
        }
    }
}
