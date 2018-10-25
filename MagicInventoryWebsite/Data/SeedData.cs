/* This code was taken from 2018_Semester1_Week7\Lecture6\Data */
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using MagicInventoryWebsite.Data;

using Microsoft.AspNetCore.Identity;

namespace MagicInventoryWebsite.Models
{
    public static class SeedData
    {
        public static async Task InitializeLogin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[]
                {MagicConstants.CustomerRole, MagicConstants.FranchiseHolderRole, MagicConstants.OwnerRole};
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = role });
                }
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            await InitializeLoginDetails(userManager);


        }

        //this method is also called in the Register method to authomatically add role to users on Registration
        public static async Task InitializeLoginDetails(UserManager<ApplicationUser> userManager)
        {
           
            await EnsureUserHasRole(userManager, "customer@example.com", MagicConstants.CustomerRole);
            //the email address was added as an additional parameter in the Store class to allow each FranchiseHolder to be assosciated with a Store
            await EnsureUserHasRole(userManager, "franchiseholder@example.com", MagicConstants.FranchiseHolderRole);
            await EnsureUserHasRole(userManager, "fh2@example.com", MagicConstants.FranchiseHolderRole);
            await EnsureUserHasRole(userManager, "fh3@example.com", MagicConstants.FranchiseHolderRole);
            await EnsureUserHasRole(userManager, "fh4@example.com", MagicConstants.FranchiseHolderRole);
            await EnsureUserHasRole(userManager, "fh5@example.com", MagicConstants.FranchiseHolderRole);


            await EnsureUserHasRole(userManager, "s3543729@student.rmit.edu.au", MagicConstants.OwnerRole);
            await EnsureUserHasRole(userManager, "owner@example.com", MagicConstants.OwnerRole);


        }



        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MagicInventoryContext(
                    serviceProvider.GetRequiredService<DbContextOptions<MagicInventoryContext>>()))
            {
                // Look for any products.
                if (context.Products.Any())
                {
                    return; // DB has been seeded.
                }

                var products = new[]
                {
                    new Product
                    {
                        Name = "Rabbit",
                        Price = 10

                    },
                    new Product
                    {
                        Name = "Hat",
                        Price = 1
                    },
                    new Product
                    {
                        Name = "Svengali Deck",
                        Price = 30
                    },
                    new Product
                    {
                        Name = "Floating Hankerchief",
                        Price = 20

                    },
                    new Product
                    {
                        Name = "Wand",
                        Price = 5

                    },
                    new Product
                    {
                        Name = "Broomstick",
                        Price = 50
                    },
                    new Product
                    {
                        Name = "Bang Gun",
                        Price = 25
                    },
                    new Product
                    {
                        Name = "Cloak of Invisibility",
                        Price = 975
                    },
                    new Product
                    {
                        Name = "Elder Wand",
                        Price = 975
                    },
                    new Product
                    {
                        Name = "Resurrection Stone",
                        Price = 975
                    }
                };

                context.Products.AddRange(products);
                var i = 0;
                context.OwnerInventory.AddRange(
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 20
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 50
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 100
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 150
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 40
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 10
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 5
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 0
                    },
                    new OwnerInventory
                    {
                        Product = products[i++],
                        StockLevel = 0
                    },
                    new OwnerInventory
                    {
                        Product = products[i],
                        StockLevel = 0
                    }
                );

                i = 0;
                var stores = new[]
                {
                             new Store
                             {
                                 User = "franchiseholder@example.com",
                                 Name = "Melbourne CBD",
                                 StoreInventory =
                                 {
                                     new StoreInventory
                                     {
                                         Product = products[i++],
                                         StockLevel = 15
                                     },
                                     new StoreInventory
                                     {
                                         Product = products[i++],
                                         StockLevel = 10
                                     },
                                     new StoreInventory
                                     {
                                         Product = products[i++],
                                         StockLevel = 5
                                     },
                                     new StoreInventory
                                     {
                                         Product = products[i++],
                                         StockLevel = 5
                                     },
                                     new StoreInventory
                                     {
                                         Product = products[i++],
                                         StockLevel = 5
                                     },
                                     new StoreInventory
                                     {
                                         Product = products[i++],
                                         StockLevel = 5
                                     },
                                     new StoreInventory
                                     {
                                         Product = products[i++],
                                         StockLevel = 5
                                     },
                                     new StoreInventory
                                     {
                                         Product = products[i++],
                                         StockLevel = 1
                                     },
                                     new StoreInventory
                                     {
                                         Product = products[i++],
                                         StockLevel = 1
                                     },
                                     new StoreInventory
                                     {
                                         Product = products[i],
                                         StockLevel = 1
                                     },
                                 }
                             },
                             new Store
                             {
                                 User = "fh2@example.com",
                                 Name = "North Melbourne",
                                 StoreInventory =
                                 {
                                     new StoreInventory
                                     {
                                         Product = products[0],
                                         StockLevel = 5
                                     }
                                 }
                             },
                             new Store
                             {
                                 User = "fh3@example.com",
                                 Name = "East Melbourne",
                                 StoreInventory =
                                 {
                                     new StoreInventory
                                     {
                                         Product = products[1],
                                         StockLevel = 5
                                     }
                                 }
                             },
                             new Store
                             {
                                 User = "fh4@example.com",
                                 Name = "South Melbourne",
                                 StoreInventory =
                                 {
                                     new StoreInventory
                                     {
                                         Product = products[2],
                                         StockLevel = 5
                                     }
                                 }
                             },
                             new Store
                             {
                                 User = "fh5@example.com",
                                 Name = "West Melbourne"
                             }
                         };
                
                context.Stores.AddRange(stores);

                context.SaveChanges();
            }
        }

        private static async Task EnsureUserHasRole(
            UserManager<ApplicationUser> userManager, string userName, string role)
        {
            var user = await userManager.FindByNameAsync(userName);
            // of the user exists and  is not in the required role
            if (user != null && !await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }




}
