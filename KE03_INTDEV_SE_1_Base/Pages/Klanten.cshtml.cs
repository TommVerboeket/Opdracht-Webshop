using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class KlantenModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;

        public IList<Customer> Customers { get; set; }

        public KlantenModel(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            Customers = new List<Customer>();
        }

        public void OnGet()
        {
            Customers = IndexModel.GetCustomersStatic(_customerRepository);
        }
    }
}
