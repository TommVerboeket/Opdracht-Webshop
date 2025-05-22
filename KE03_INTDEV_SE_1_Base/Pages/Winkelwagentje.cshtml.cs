using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class WinkelwagentjeModel : PageModel
    {
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        public class WinkelwagenItem
        {
            public Product Product { get; set; }
            public int Aantal { get; set; }
        }

        public static List<List<WinkelwagenItem>> Bestellingen = new();

        public List<WinkelwagenItem> Winkelwagen { get; set; } = new();
        [BindProperty]
        public int ProductId { get; set; }
        [BindProperty]
        public string Actie { get; set; }
        public string Melding { get; set; }

        public void OnGet()
        {
            LaadWinkelwagen();
        }

        public IActionResult OnPost()
        {
            LaadWinkelwagen();
            if (Actie == "meer")
            {
                var item = Winkelwagen.FirstOrDefault(i => i.Product.Id == ProductId);
                if (item != null) item.Aantal++;
                Melding = "Aantal verhoogd.";
            }
            else if (Actie == "minder")
            {
                var item = Winkelwagen.FirstOrDefault(i => i.Product.Id == ProductId);
                if (item != null && item.Aantal > 1) item.Aantal--;
                else if (item != null) Winkelwagen.Remove(item);
                Melding = "Aantal verlaagd.";
            }
            else if (Actie == "verwijder")
            {
                var item = Winkelwagen.FirstOrDefault(i => i.Product.Id == ProductId);
                if (item != null) Winkelwagen.Remove(item);
                Melding = "Product verwijderd.";
            }
            else if (Actie == "bestel")
            {
                if (Winkelwagen.Any())
                {
                    Bestellingen.Add(Winkelwagen.Select(i => new WinkelwagenItem
                    {
                        Product = i.Product,
                        Aantal = i.Aantal
                    }).ToList());
                    Winkelwagen.Clear();
                    Melding = "Bestelling geplaatst!";
                }
                else
                {
                    Melding = "Winkelwagentje is leeg.";
                }
            }
            SlaWinkelwagenOp();
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

        private void LaadWinkelwagen()
        {
            if (HttpContext.Session.TryGetValue("Cart", out var bytes))
            {
                Winkelwagen = JsonSerializer.Deserialize<List<WinkelwagenItem>>(bytes) ?? new();
            }
        }

        private void SlaWinkelwagenOp()
        {
            HttpContext.Session.Set("Cart", JsonSerializer.SerializeToUtf8Bytes(Winkelwagen));
        }
    }
}
