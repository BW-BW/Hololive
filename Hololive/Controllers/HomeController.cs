using Microsoft.AspNetCore.Mvc;
using Hololive.Models;
using Hololive.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Amazon.S3.Model;
using Amazon.S3;
using System.Text;

using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Hololive.Controllers
{
    public class HomeController : Controller
    {

        private readonly HololiveContext _context;

        private const string topicARN = "arn:aws:sns:us-east-1:576578684530:HololiveAnnouncement";

        public HomeController(HololiveContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //return View();
            int? customerIDFlag = HttpContext.Session.GetInt32("CustomerID");

            if (customerIDFlag.HasValue)
            {
                List<Voucher> voucherList = await _context.Voucher.ToListAsync();

                //return View();
                return View("Login", voucherList);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerID");
            return RedirectToAction("Index", "Home");
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

                //subscribe SNS topic
                await processSubscribtion(customer.CustomerEmail);

                return RedirectToAction("Index", "Home");
            }

            return View("Register", customer);
        }

        //get AWS keys from appsettings.json
        private List<string> getKeys()
        {
            List<string> keys = new List<string>();

            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
            IConfiguration configure = builder.Build(); //build the json file

            //2. read the info from json using configure instance
            keys.Add(configure["Values:key1"]);
            keys.Add(configure["Values:key2"]);
            keys.Add(configure["Values:key3"]);

            return keys;
        }

        //process the subscription in SNS topic
        public async Task<IActionResult> processSubscribtion(string email)
        {
            //start connection
            List<string> values = getKeys();
            AmazonSimpleNotificationServiceClient agent = new AmazonSimpleNotificationServiceClient(values[0], values[1], values[2], RegionEndpoint.USEast1);

            try
            {
                SubscribeRequest request = new SubscribeRequest
                {
                    TopicArn = topicARN,
                    Protocol = "Email",
                    Endpoint = email
                };
                SubscribeResponse response = await agent.SubscribeAsync(request);
                //ViewBag.subscribtionSuccessID = response.ResponseMetadata.RequestId;
                return View("Index", "Home");
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                return BadRequest(ex.Message);
            }
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

        //profile function
        public async Task<IActionResult> Profile()
        {
            int? customerID = HttpContext.Session.GetInt32("CustomerID");

            Customer customers = await _context.Customer
                .FirstOrDefaultAsync(s => s.CustomerID == customerID.Value);

            return View(customers);
        }

        public async Task<IActionResult> NewProduct()
        {
            int maxTransactionID = await _context.Voucher
                            .MaxAsync(s => (int?)s.VoucherID) ?? 0;

            Voucher voucher = await _context.Voucher
                .FirstOrDefaultAsync(s => s.VoucherID == maxTransactionID);

            return View(voucher);
        }

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