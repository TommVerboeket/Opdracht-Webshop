using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class WinkelwagentjeModel : PageModel
    {
        public class CartItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }

        public static List<List<CartItem>> Bestellingen = new();

        public List<CartItem> Cart { get; set; } = new();
        [BindProperty]
        public int ProductId { get; set; }
        [BindProperty]
        public string Actie { get; set; }
        public string Melding { get; set; }

        public void OnGet()
        {
            LoadCart();
        }

        public IActionResult OnPost()
        {
            LoadCart();
            if (Actie == "meer")
            {
                var item = Cart.FirstOrDefault(i => i.Product.Id == ProductId);
                if (item != null) item.Quantity++;
                Melding = "Aantal verhoogd.";
            }
            else if (Actie == "minder")
            {
                var item = Cart.FirstOrDefault(i => i.Product.Id == ProductId);
                if (item != null && item.Quantity > 1) item.Quantity--;
                else if (item != null) Cart.Remove(item);
                Melding = "Aantal verlaagd.";
            }
            else if (Actie == "verwijder")
            {
                var item = Cart.FirstOrDefault(i => i.Product.Id == ProductId);
                if (item != null) Cart.Remove(item);
                Melding = "Product verwijderd.";
            }
            else if (Actie == "bestel")
            {
                if (Cart.Any())
                {
                    Bestellingen.Add(Cart.Select(i => new CartItem
                    {
                        Product = i.Product,
                        Quantity = i.Quantity
                    }).ToList());
                    Cart.Clear();
                    Melding = "Bestelling geplaatst!";
                }
                else
                {
                    Melding = "Winkelwagentje is leeg.";
                }
            }
            SaveCart();
            return RedirectToPage(new { melding = Melding });
        }

        public override void OnPageHandlerExecuted(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutedContext context)
        {
            base.OnPageHandlerExecuted(context);
            if (Request.Query.ContainsKey("melding"))
            {
                Melding = Request.Query["melding"];
            }
        }

        private void LoadCart()
        {
            if (HttpContext.Session.TryGetValue("Cart", out var bytes))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                Cart = JsonSerializer.Deserialize<List<CartItem>>(bytes, options) ?? new();
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
