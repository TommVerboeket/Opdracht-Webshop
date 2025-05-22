using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class BestelgeschiedenisModel : PageModel
    {
        private readonly MatrixIncDbContext _db;

        public BestelgeschiedenisModel(MatrixIncDbContext db)
        {
            _db = db;
        }

        public List<Order> Bestellingen { get; set; } = new();

        public void OnGet()
        {
            Bestellingen = _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Products)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
    }
}
