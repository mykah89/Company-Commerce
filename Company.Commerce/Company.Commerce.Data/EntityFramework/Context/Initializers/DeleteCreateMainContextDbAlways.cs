using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data.EntityFramework.Context.Initializers
{
    public class DeleteCreateMainContextDbAlways : MainContextInitializer
    {
        public override void InitializeDatabase(MainContext context)
        {
            if (context.Database.Exists())
                context.Database.Delete();

            String dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as String;

            List<String> dbPaths = new List<String>();

            dbPaths.AddRange(Directory.GetFiles(dataDirectory, "*.mdf"));
            dbPaths.AddRange(Directory.GetFiles(dataDirectory, "*.ldf"));

            dbPaths.ForEach(p => File.Delete(p));

            //Creates database if it does not exist.
            base.InitializeDatabase(context);

            Seed(context);
        }

        protected override void Seed(MainContext context)
        {
            base.Seed(context);

            User user = new User()
            {
                AccountConfirmed = true,
                EmailAddress = "admins@domain.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admins", 12),
                PasswordVerificationTokenExpiration = DateTime.Now,
                Username = "admins"
            };

            context.Set<User>().Add(user);

            for (int i = 1; i < 6; i++)
            {
                Category category = new Category()
                {
                    Name = "Category" + i.ToString()
                };

                context.Set<Category>().Add(category);
            }

            Image productImage = new Image()
            {
                ImagePath = "~/Content/Images/Products/DIY-Hammer-icon.png"
            };

            Image productImage2 = new Image()
            {
                ImagePath = "~/Content/Images/Products/download.jpg"
            };

            Image categoryImage = new Image()
            {

                ImagePath = "~/Content/Images/Categories/categoryImage.png"
            };

            context.Set<Image>().Add(productImage);

            context.Set<Image>().Add(productImage2);

            context.Set<Image>().Add(categoryImage);

            context.SaveChanges();

            Int32 productCount = 1;

            foreach (var category in context.Set<Category>())
            {
                CategoryImageXref categoryImageXref = new CategoryImageXref()
                {
                    Category = category,
                    Image = categoryImage
                };

                category.AssociatedImages.Add(categoryImageXref);

                for (int i = 1; i < 21; i++)
                {
                    Product product = new Product()
                    {
                        IsActive = true,
                        Name = "SomeProduct" + productCount,
                        Price = new Random().Next(1, 3) * i,
                        UnitsInStock = 10
                    };

                    context.Set<Product>().Add(product);

                    ProductCategoryXref productCategoryXref = new ProductCategoryXref()
                    {
                        Category = category,
                        Product = product,
                        IsDefault = true
                    };

                    product.AssociatedCategories.Add(productCategoryXref);

                    ProductImageXref productImageXref = new ProductImageXref()
                    {
                        Product = product,
                        Image = productImage,
                        IsDefault = true
                    };

                    ProductImageXref productImageXref2 = new ProductImageXref()
                    {
                        Product = product,
                        Image = productImage2,
                        IsDefault = false
                    };

                    product.AssociatedImages.Add(productImageXref);

                    product.AssociatedImages.Add(productImageXref2);

                    productCount++;
                }
            }

            context.SaveChanges();
        }
    }
}
