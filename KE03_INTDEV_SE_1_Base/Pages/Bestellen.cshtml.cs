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
          
            var klantJson = HttpContext.Session.GetString("KlantGegevens");
            var winkelwagenJson = HttpContext.Session.Get("Cart");
            if (klantJson != null && winkelwagenJson != null)
            {
                var klant = JsonSerializer.Deserialize<KlantGegevens>(klantJson);
                var winkelwagen = JsonSerializer.Deserialize<List<CartItem>>(winkelwagenJson);

                if (klant != null && winkelwagen != null && winkelwagen.Any())
                {
                    
                    var customer = _db.Customers.FirstOrDefault(c => c.Name == klant.Naam && c.Address == klant.Adres);
                    if (customer == null)
                    {
                        customer = new Customer { Name = klant.Naam, Address = klant.Adres, Active = true };
                        _db.Customers.Add(customer);
                        _db.SaveChanges();
                    }

        
                    var order = new Order
                    {
                        OrderDate = DateTime.Now,
                        CustomerId = customer.Id,
                        Customer = customer
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
                    Melding = "Bestelling niet gelukt.";
                }
            }
            else
            {
                Melding = "Bestelling niet gelukt.";
            }
        }

        public class KlantGegevens
        {
            public string Naam { get; set; }
            public string Adres { get; set; }
            public string Betaalmethode { get; set; }
        }

        public class CartItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }
    }
}
