using API.Entities;
using Microsoft.AspNetCore.Identity;
using Stripe;
using Product = API.Entities.Product;

namespace API.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(StoreContext context, UserManager<User> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new User
                {
                    UserName = "bob",
                    Email = "bob@test.com"
                };

                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");

                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@test.com"
                };

                await userManager.CreateAsync(admin, "Pa$$w0rd");
                await userManager.AddToRolesAsync(admin, new[] {"Admin"});
                
                var staff1 = new User
                {
                    UserName = "staff1",
                    Email = "staff1@test.com"
                };

                await userManager.CreateAsync(staff1, "Pa$$w0rd");
                await userManager.AddToRolesAsync(staff1, new[] {"Staff"});
                
                var staff2 = new User
                {
                    UserName = "staff2",
                    Email = "staff2@test.com"
                };

                await userManager.CreateAsync(staff2, "Pa$$w0rd");
                await userManager.AddToRolesAsync(staff2, new[] {"Staff"});
                
                var staff3 = new User
                {
                    UserName = "staff3",
                    Email = "staff3@test.com"
                };

                await userManager.CreateAsync(staff3, "Pa$$w0rd");
                await userManager.AddToRolesAsync(staff3, new[] {"Staff"});
                
                var staff4 = new User
                {
                    UserName = "staff4",
                    Email = "staff4@test.com"
                };

                await userManager.CreateAsync(staff4, "Pa$$w0rd");
                await userManager.AddToRolesAsync(staff4, new[] {"Staff"});
            }
            

            if (context.Products.Any()) return;
            
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Angular Speedster Board 2000",
                    Description =
                        "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                   
                    Price = 20000, 
                    BasePrice = 10000,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1713418361/cvpw7xee1qz7hjrtqmny.webp",
                    Brand = "VietNameSupleMent",
                    Type = "Pre-Workout Supplements",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Green Angular Board 3000",
                    Description = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.",
                    Price = 15000,
                    BasePrice = 7500,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1714321302/sadc3cvb4tqomcgznykm.webp",
                    Brand = "Nutrabolics",
                    Type = "Protein Supplements",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Core Board Speed Rush 3",
                    Description =
                        "Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy.",
                    Price = 18000,
                    BasePrice = 9000,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1714321345/s9wdshgpr3wyzhzwegcn.jpg",
                    Brand = "OptiumNutirtion",
                    Type = "Casein Protein",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Net Core Super Board",
                    Description =
                        "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.",
                    Price = 30000,
                    BasePrice = 15000,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1714321373/juosat3ieqo9mjaegugi.webp",
                    Brand = "OptiumNutirtion",
                    Type = "Casein Protein",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "React Board Super Whizzy Fast",
                    Description =
                        "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 25000,
                    BasePrice = 12500,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1713595146/rxu4nxrcmjxyhnawkf75.jpg",
                    Brand = "BioTechUsa",
                    Type = "Casein Protein",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Typescript Entry Board",
                    Description =
                        "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 12000,
                    BasePrice = 6000,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1713589262/h9oermilpsf9garmgxdw.webp",
                    Brand = "Nutrabolics",
                    Type = "Casein Protein",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Core Blue Hat",
                    Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 1000,
                    BasePrice = 500,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1714321449/oqmy8wnde4mo5chlsx37.jpg",
                    Brand = "OptiumNutirtion",
                    Type = "Mass Gainers",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Green React Woolen Hat",
                    Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 8000,
                    BasePrice = 4000,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1714381377/na5c8jn1h6zwxxsqtuv5.jpg",
                    Brand = "BioTechUsa",
                    Type = "Mass Gainers",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Purple React Woolen Hat",
                    Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 1500,
                    BasePrice = 750,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1714321345/s9wdshgpr3wyzhzwegcn.jpg",
                    Brand = "BioTechUsa",
                    Type = "Mass Gainers",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Blue Code Gloves",
                    Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 1800,
                    BasePrice = 900,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1714321302/sadc3cvb4tqomcgznykm.webp",
                    Brand = " Mutant",
                    Type = "Creatine",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Green Code Gloves",
                    Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 1500,
                    BasePrice = 900,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1713595113/llosrhbu3mpx8a1kfpzx.jpg",
                    Brand = " Mutant",
                    Type = "HMB (Beta-Hydroxy Beta-Methylbutyrate)",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Purple React Gloves",
                    Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 1600,
                    BasePrice = 800,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1713595113/llosrhbu3mpx8a1kfpzx.jpg",
                    Brand = "BioTechUsa",
                    Type = "Creatine",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Green React Gloves",
                    Description =
                        "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 1400,
                    BasePrice = 700,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1713595113/llosrhbu3mpx8a1kfpzx.jpg",
                    Brand = "BioTechUsa",
                    Type = "Creatine",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Redis Red Boots",
                    Description =
                        "Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy.",
                    Price = 25000,
                    BasePrice = 12500,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1714321302/sadc3cvb4tqomcgznykm.webp",
                    Brand = "The Curse ",
                    Type = "PRE-WORKOUT ",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Core Red Boots",
                    Description =
                        "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.",
                    Price = 18999,
                    BasePrice = 9000,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1713589262/h9oermilpsf9garmgxdw.webp",
                    Brand = "OptiumNutirtion",
                    Type = "Glutamine",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Core Purple Boots",
                    Description =
                        "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.",
                    Price = 19999,
                    BasePrice = 1000,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1713595113/llosrhbu3mpx8a1kfpzx.jpg",
                    Brand = "OptiumNutirtion",
                    Type = "Glutamine",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Angular Purple Boots",
                    Description = "Aenean nec lorem. In porttitor. Donec laoreet nonummy augue.",
                    Price = 15000,
                    BasePrice = 75000,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1713595113/llosrhbu3mpx8a1kfpzx.jpg",
                    Brand = "Nutrabolics",
                    Type = "Glutamine",
                    QuantityInStock = 100
                },
                new Product
                {
                    Name = "Angular Blue Boots",
                    Description =
                        "Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy.",
                    Price = 18000,
                    BasePrice = 9000,
                    PictureUrl = "https://res.cloudinary.com/dydd7mqle/image/upload/v1712313387/nz7xxcnrgtpy5lhaoewz.webp",
                    Brand = "Nutrabolics",
                    Type = "Glutamine",
                    QuantityInStock = 100
                },
            };
            
            foreach (var product in products)
            {
                context.Products.Add(product);
            }
            
            context.SaveChanges();
        }
    }
}