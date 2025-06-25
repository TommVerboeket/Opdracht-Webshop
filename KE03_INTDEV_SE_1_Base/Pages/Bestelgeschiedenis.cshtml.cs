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

        public List<OrderViewModel> Bestellingen { get; set; } = new();

        public void OnGet()
        {
            var orders = _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Products)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            Bestellingen = orders.Select(order => new OrderViewModel
            {
                OrderDate = order.OrderDate,
                CustomerName = order.Customer?.Name ?? "",
                CustomerAddress = order.Customer?.Address ?? "",
                Products = order.Products
                    .GroupBy(p => p.Id)
                    .Select(g => new ProductViewModel
                    {
                        Name = g.First().Name,
                        Price = g.First().Price,
                        Quantity = g.Count()
                    }).ToList()
            }).ToList();
        }

        public class OrderViewModel
        {
            public System.DateTime OrderDate { get; set; }
            public string CustomerName { get; set; }
            public string CustomerAddress { get; set; }
            public List<ProductViewModel> Products { get; set; }
        }

        public class ProductViewModel
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}
