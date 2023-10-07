using Microsoft.AspNetCore.Mvc;
using Hololive.Models;
using Hololive.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Hololive.Controllers
{
    public class HomeController : Controller
    {

        private readonly HololiveContext _context;

        public HomeController(HololiveContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login2(string username, string password)
        {
            //if row = find, get the whole row, put it in global variable.

            return View();
        }

        public async Task<IActionResult> Login(string email, string password)
        {
            List<Customer> customers = await _context.Customer.ToListAsync();

            //filter before display

            customers = customers.Where(s => s.CustomerEmail.Contains(email)).ToList();
                
            if (customers.Count > 0)
            {
                return View(customers);
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //function 2: Insert data to table
        [HttpPost]
        [ValidateAntiForgeryToken]//avoid cross-site attack
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customer.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View("Register", customer);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}