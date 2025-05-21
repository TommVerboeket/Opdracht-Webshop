using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class ProductenModel : PageModel
    {
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string ImageUrl { get; set; }
            public string Category { get; set; }
        }

        public class CartItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }

        public List<Product> Products { get; set; } = new()
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

        public List<CartItem> Cart { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }
        public List<Product> FilteredProducts { get; set; }
        public bool ShowAddedMessage { get; set; }

        public void OnGet()
        {
            LoadCart();
            FilteredProducts = FilterProducts();
            ShowAddedMessage = false;
        }

        public IActionResult OnPostAddToCart(int productId)
        {
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
            // Toon melding na toevoegen
            return RedirectToPage(new { search = Search, added = true });
        }

        public override void OnPageHandlerExecuted(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutedContext context)
        {
            base.OnPageHandlerExecuted(context);
            if (Request.Query.ContainsKey("added"))
            {
                ShowAddedMessage = true;
            }
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
                Cart = JsonSerializer.Deserialize<List<CartItem>>(cartBytes) ?? new();
            }
        }

        private void SaveCart()
        {
            HttpContext.Session.Set("Cart", JsonSerializer.SerializeToUtf8Bytes(Cart));
        }
    }
}
