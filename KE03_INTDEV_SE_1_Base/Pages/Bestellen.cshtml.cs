using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class BestellenModel : PageModel
    {
        private readonly MatrixIncDbContext _db;

        public BestellenModel(MatrixIncDbContext db)
        {
            _db = db;
        }

        public string Melding { get; set; }
        public bool Besteld { get; set; }

        public void OnGet()
        {
            Besteld = false;
        }

        public void OnPost()
        {
            Besteld = false;

            // Haal winkelwagen uit sessie
            var winkelwagenJson = HttpContext.Session.Get("Cart");
            var winkelwagen = winkelwagenJson != null
                ? JsonSerializer.Deserialize<List<CartItem>>(winkelwagenJson)
                : null;

            if (winkelwagen != null && winkelwagen.Any())
            {
                // Dummy klant (of haal uit sessie als je klantgegevens hebt)
                var klantNaam = "Gast";
                var klantAdres = "Onbekend";
                var klant = _db.Customers.FirstOrDefault(c => c.Name == klantNaam && c.Address == klantAdres);
                if (klant == null)
                {
                    klant = new Customer { Name = klantNaam, Address = klantAdres, Active = true };
                    _db.Customers.Add(klant);
                    _db.SaveChanges();
                }

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerId = klant.Id,
                    Customer = klant,
                    Products = new List<Product>()
                };

                foreach (var item in winkelwagen)
                {
                    var product = _db.Products.FirstOrDefault(p => p.Id == item.Product.Id);
                    if (product != null)
                    {
                        for (int i = 0; i < item.Quantity; i++)
                        {
                            order.Products.Add(product);
                        }
                    }
                }

                _db.Orders.Add(order);
                _db.SaveChanges();

                HttpContext.Session.Remove("Cart");
                Besteld = true;
                Melding = "Bestelling afgerond!";
            }
            else
            {
                Melding = "Bestelling niet gelukt. Winkelwagen is leeg.";
            }
        }

        public class CartItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }
    }
}
