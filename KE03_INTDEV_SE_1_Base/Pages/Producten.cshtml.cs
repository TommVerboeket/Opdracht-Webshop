using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class ProductenModel : PageModel
    {
        private readonly MatrixIncDbContext _context;

        public ProductenModel(MatrixIncDbContext context)
        {
            _context = context;
        }

        // CartItem class for the cart
        public class CartItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }

        public List<Product> Products { get; set; } = new();

        public List<Product> FilteredProducts { get; set; } = new();

        public List<CartItem> Cart { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public bool ShowAddedMessage { get; set; }

        public void OnGet()
        {
            // Load products from database
            Products = _context.Products.ToList();

            // Load cart from session
            LoadCart();

            // Filter products by search term
            FilteredProducts = FilterProducts();

            // Check if something was just added
            if (Request.Query.ContainsKey("added"))
            {
                ShowAddedMessage = true;
            }
        }

        public IActionResult OnPostAddToCart(int productId)
        {
            Products = _context.Products.ToList();
            LoadCart();

            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                var item = Cart.FirstOrDefault(i => i.Product.Id == productId);
                if (item != null)
                {
                    item.Quantity++;
                }
                else
                {
                    // Always set Quantity = 1 for new items
                    Cart.Add(new CartItem { Product = product, Quantity = 1 });
                }
            }

            SaveCart();
            return RedirectToPage(new { search = Search, added = true });
        }

        private List<Product> FilterProducts()
        {
            if (string.IsNullOrWhiteSpace(Search))
                return Products;

            var searchLower = Search.ToLower();
            return Products.Where(p =>
                p.Name.ToLower().Contains(searchLower) ||
                p.Category.ToLower().Contains(searchLower)
            ).ToList();
        }

        private void LoadCart()
        {
            if (HttpContext.Session.TryGetValue("Cart", out var cartBytes))
            {
                // Gebruik JsonSerializerOptions om case-insensitive te deserialiseren
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                Cart = JsonSerializer.Deserialize<List<CartItem>>(cartBytes, options) ?? new();
            }
            else
            {
                Cart = new List<CartItem>();
            }
        }

        private void SaveCart()
        {
            HttpContext.Session.Set("Cart", JsonSerializer.SerializeToUtf8Bytes(Cart));
        }
    }
}
