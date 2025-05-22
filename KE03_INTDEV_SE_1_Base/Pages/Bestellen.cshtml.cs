using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class BestellenModel : PageModel
    {
        public string Melding { get; set; }
        public bool Besteld { get; set; }

        // Deze klassen moeten overeenkomen met de rest van je project
        public class KlantGegevens
        {
            public string Naam { get; set; }
            public string Adres { get; set; }
            public string Betaalmethode { get; set; }
        }

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

        public class Bestelling
        {
            public KlantGegevens Klant { get; set; }
            public List<WinkelwagenItem> Producten { get; set; }
        }

        // Simpele "database" voor bestellingen
        public static List<Bestelling> Bestellingen = new();

        public void OnGet()
        {
            Besteld = false;
        }

        public void OnPost()
        {
            Besteld = false;
            // Haal klantgegevens op uit sessie
            var klantJson = HttpContext.Session.GetString("KlantGegevens");
            var winkelwagenJson = HttpContext.Session.Get("Cart");
            if (klantJson != null && winkelwagenJson != null)
            {
                var klant = JsonSerializer.Deserialize<KlantGegevens>(klantJson);
                var winkelwagen = JsonSerializer.Deserialize<List<WinkelwagenItem>>(winkelwagenJson);

                if (klant != null && winkelwagen != null && winkelwagen.Any())
                {
                    // Sla bestelling + klantgegevens op in de "database"
                    Bestellingen.Add(new Bestelling
                    {
                        Klant = klant,
                        Producten = winkelwagen
                    });
                    // Leeg winkelwagen
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
    }
}
