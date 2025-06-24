using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class MatrixIncDbInitializer
    {
        public static void Initialize(MatrixIncDbContext context)
        {
            // Look for any customers.
            if (context.Customers.Any())
            {
                return;   // DB has been seeded
            }

            // TODO: Hier moet ik nog wat namen verzinnen die betrekking hebben op de matrix.
            // - Denk aan de m3 boutjes, moertjes en ringetjes.
            // - Denk aan namen van schepen
            // - Denk aan namen van vliegtuigen            
            var customers = new Customer[]
            {
                new Customer { Name = "Neo", Address = "123 Elm St" , Active=true},
                new Customer { Name = "Morpheus", Address = "456 Oak St", Active = true },
                new Customer { Name = "Trinity", Address = "789 Pine St", Active = true }
            };
            context.Customers.AddRange(customers);

           

            var products = new Product[]
            {
                new Product { Id = 1, Name = "Remschijven", Description = "Hoogwaardige remschijven voor optimale veiligheid.", Price = 49.95m, ImageUrl = "/images/remschijf.jpg", Category = "Remmen" },
                new Product { Id = 2, Name = "Olie filter", Description = "Betrouwbare oliefilter voor een lange levensduur van je motor.", Price = 12.50m, ImageUrl = "/images/oliefilter.jpg", Category = "Motor" },
                new Product { Id = 3, Name = "Bougieset", Description = "Complete set bougies voor een soepele ontsteking.", Price = 29.99m, ImageUrl = "/images/bougieset.jpg", Category = "Motor" },
                new Product { Id = 4, Name = "Ruitenwisser", Description = "Heldere ruiten met deze duurzame ruitenwissers.", Price = 15.00m, ImageUrl = "/images/ruitenwisser.jpg", Category = "Exterieur" },
                new Product { Id = 5, Name = "Accu", Description = "Krachtige accu voor betrouwbare starts.", Price = 89.00m, ImageUrl = "/images/accu.jpg", Category = "Elektrisch" },
                new Product { Id = 6, Name = "Koplamp", Description = "Heldere koplamp voor maximale zichtbaarheid.", Price = 39.95m, ImageUrl = "/images/koplamp.jpg", Category = "Verlichting" },
                new Product { Id = 7, Name = "Luchtfilter", Description = "Efficiënte luchtfilter voor optimale motorprestaties.", Price = 17.50m, ImageUrl = "/images/luchtfilter.jpg", Category = "Motor" },
                new Product { Id = 8, Name = "Banden", Description = "All-season banden voor veilig rijgedrag.", Price = 75.00m, ImageUrl = "/images/band.jpg", Category = "Wielen" }
            };
            context.Products.AddRange(products);

            var parts = new Part[]
            {
                new Part { Name = "Tandwiel", Description = "Overdracht van rotatie in bijvoorbeeld de motor of luikmechanismen"},
                new Part { Name = "M5 Boutje", Description = "Bevestiging van panelen, buizen of interne modules"},
                new Part { Name = "Hydraulische cilinder", Description = "Openen/sluiten van zware luchtsluizen of bewegende onderdelen"},
                new Part { Name = "Koelvloeistofpomp", Description = "Koeling van de motor of elektronische systemen."}
            };
            context.Parts.AddRange(parts);

            var orders = new Order[]
           {
                new Order { Customer = customers[0], OrderDate = DateTime.Parse("2021-01-01"), Products = new List<Product>{products[1] }},
                new Order { Customer = customers[0], OrderDate = DateTime.Parse("2021-02-01"), Products = new List<Product>{products[2] }},
                new Order { Customer = customers[1], OrderDate = DateTime.Parse("2021-02-01"), Products = new List<Product>{products[3] }},
                new Order { Customer = customers[2], OrderDate = DateTime.Parse("2021-03-01"), Products = new List<Product>{products[4], products[2] }}
           };
            context.Orders.AddRange(orders);

            context.SaveChanges();

            context.Database.EnsureCreated();
        }
    }
}
