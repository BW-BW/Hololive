using Microsoft.AspNetCore.Mvc;
using Hololive.Models;
using Hololive.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Amazon.S3.Model;
using Amazon.S3;
using System.Text;

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

        public async Task<IActionResult> Login(string email, string password)
        {
            Customer customers = await _context.Customer.FirstOrDefaultAsync(s => s.CustomerEmail == email && s.CustomerPassword == password);
            
            //Customer customers = await _context.Customer.FirstOrDefaultAsync(s => s.CustomerEmail == _customerEmail);
            if (customers != null)
            {
                HttpContext.Session.SetInt32("CustomerID", customers.CustomerID);
                HttpContext.Session.SetString("CustomerName", customers.CustomerName);

                // Successful login
                List<Voucher> voucherList = await _context.Voucher.ToListAsync();

                return View(voucherList);// Redirect to AdminHome action

                //return View(customers);
            } 
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //customer register function
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

        //buy function
        public async Task<IActionResult> BuyVoucher(Transactions transactions, int? VoucherID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var voucher = await _context.Voucher.FindAsync(VoucherID);
                    int? customerID = HttpContext.Session.GetInt32("CustomerID");

                    if (customerID.HasValue)
                    {
                        transactions.CustomerID = customerID.Value;
                        transactions.GiftcardCode = GenerateRandomCode(); //randomized
                        transactions.VoucherID = voucher.VoucherID;

                        _context.Transactions.Add(transactions);
                        await _context.SaveChangesAsync();

                        int maxTransactionID = await _context.Transactions
                            .Where(s => s.CustomerID == customerID.Value)
                            .MaxAsync(s => (int?)s.TransactionID) ?? 0;

                        Transactions trans = await _context.Transactions
                            .FirstOrDefaultAsync(s => s.CustomerID == customerID.Value && s.TransactionID == maxTransactionID);

                        return View(trans);
                        //return RedirectToAction("Login", "Home");
                    }
                    
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return View("Register", transactions);
        }

        static string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder randomStringBuilder = new StringBuilder();

            Random random = new Random();
            for (int i = 0; i < 14; i++)
            {
                if (i == 4 || i == 8)
                {
                    randomStringBuilder.Append('-');
                }
                else
                {
                    randomStringBuilder.Append(chars[random.Next(chars.Length)]);
                }
            }

            return randomStringBuilder.ToString();
        }

        //hisotry function
        /*public async Task<IActionResult> History()
        {
            List<Transactions> trans = await _context.Transactions.ToListAsync();

            //filter before display
            int? customerID = HttpContext.Session.GetInt32("CustomerID");
            trans = trans.Where(s => s.CustomerID == customerID.Value).ToList();

            return View(trans);
        }*/

        public async Task<IActionResult> History()
        {
            List<HistoryView> transactions = await _context.Transactions
                .Where(t => t.CustomerID == HttpContext.Session.GetInt32("CustomerID"))
                .Join(
                    _context.Voucher,
                    transaction => transaction.VoucherID,
                    voucher => voucher.VoucherID,
                    (transaction, voucher) => new HistoryView
                    {
                        TransactionID = transaction.TransactionID,
                        CustomerID = transaction.CustomerID,
                        VoucherID = transaction.VoucherID,
                        GiftcardCode = transaction.GiftcardCode,
                        VoucherName = voucher.VoucherName,
                        VoucherValue = voucher.VoucherValue,
                        VoucherPrice = voucher.VoucherPrice
                    })
                .ToListAsync();

            return View(transactions);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}