using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICustomerRepository _customerRepository;

        public IList<Customer> Customers { get; set; }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        public class CartItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }

        public List<Product> Products { get; set; } = new()
        {
            new Product { Id = 1, Name = "Remschijven", Price = 49.95m },
            new Product { Id = 2, Name = "Olie filter", Price = 12.50m },
            new Product { Id = 3, Name = "Bougieset", Price = 29.99m }
        };

        public List<CartItem> Cart { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            Customers = new List<Customer>();
        }

        public void OnGet()
        {
            Customers = _customerRepository.GetAllCustomers().ToList();
            LoadCart();
        }

        public IActionResult OnPostAddToCart(int productId)
        {
            Customers = _customerRepository.GetAllCustomers().ToList();
            LoadCart();

            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                var item = Cart.FirstOrDefault(i => i.Product.Id == productId);
                if (item != null)
                    item.Quantity++;
                else
                    Cart.Add(new CartItem { Product = product, Quantity = 1 });
            }

            SaveCart();
            return RedirectToPage();
        }

        private void LoadCart()
        {
            if (HttpContext.Session.TryGetValue("Cart", out var cartBytes))
            {
                Cart = JsonSerializer.Deserialize<List<CartItem>>(cartBytes) ?? new();
            }
        }

        private void SaveCart()
        {
            HttpContext.Session.Set("Cart", JsonSerializer.SerializeToUtf8Bytes(Cart));
        }
    }
}
