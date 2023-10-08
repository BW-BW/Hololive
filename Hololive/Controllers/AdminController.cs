using Microsoft.AspNetCore.Mvc;
using Hololive.Models;
using Hololive.Data;
using Microsoft.EntityFrameworkCore;

using Amazon; //for linking your AWS account
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration; //appsettings.json section
using System.IO; // input output
using Microsoft.AspNetCore.Http;

using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Hololive.Controllers
{
    public class AdminController : Controller
    {
        private const string s3BucketName = "testtest12091";

        private const string topicARN = "arn:aws:sns:us-east-1:576578684530:HololiveAnnouncement";

        private readonly HololiveContext _context;

        public AdminController(HololiveContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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

        //function for admin to broadcast
        public async Task<IActionResult> processBroadcast()
        {
            //start connection
            List<string> values = getKeys();
            AmazonSimpleNotificationServiceClient agent = new AmazonSimpleNotificationServiceClient(values[0], values[1], values[2], RegionEndpoint.USEast1);

            try
            {
                PublishRequest request = new PublishRequest
                {
                    TopicArn = topicARN,
                    Subject = "Hololive New Product",
                    Message = "Hey there, our platform just release a new type of voucher, come check it out on our website http://hololive-test.us-east-1.elasticbeanstalk.com/"
                };
                await agent.PublishAsync(request);
                return RedirectToAction("Index", "Admin");
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //add voucher to S3 and RDS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVoucher(Voucher voucher, IFormFile imagefile)
        {
            //start connection
            List<string> values = getKeys();
            AmazonS3Client s3agent = new AmazonS3Client(values[0], values[1], values[2], RegionEndpoint.USEast1);

            //3. upload image to S3 and get the URL
            try
            {
                //upload to S3
                PutObjectRequest request = new PutObjectRequest //generate the request
                {
                    InputStream = imagefile.OpenReadStream(),
                    BucketName = s3BucketName,
                    Key = "images/" + imagefile.FileName,
                    CannedACL = S3CannedACL.PublicRead
                };

                //send out the request
                await s3agent.PutObjectAsync(request);
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest("Unable to upload to S3 due to technical issue. Error message: " + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to upload to S3 due to technical issue. Error message: " + ex.Message);
            }
            voucher.VoucherLink = "https://" + s3BucketName + ".s3.amazonaws.com/images/" + imagefile.FileName;
            voucher.VoucherS3Key = imagefile.FileName;

            if (ModelState.IsValid)
            {
                //voucher.VoucherLink = "asdasd";
                _context.Voucher.Add(voucher);
                await _context.SaveChangesAsync();
                
                //send out SNS
                await processBroadcast();

                return RedirectToAction("Index", "Admin");
            }
            return View("Index", voucher);

        }

        //Delete item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deleteVoucher(int? VoucherID)
        {
            if (VoucherID == null)
            {
                return NotFound();
            }

            try
            {
                var voucher = await _context.Voucher.FindAsync(VoucherID);

                List<string> values = getKeys();
                AmazonS3Client s3agent = new AmazonS3Client(values[0], values[1], values[2], RegionEndpoint.USEast1);

                //create a delete request 
                DeleteObjectRequest deleteRequest = new DeleteObjectRequest
                {
                    BucketName = s3BucketName,
                    Key = voucher.VoucherS3Key
                };

                await s3agent.DeleteObjectAsync(deleteRequest);

                _context.Voucher.Remove(voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult AddNewVoucher()
        {
            return View();
        }


        //login function + show all listing
        public async Task<IActionResult> AdminHome(string username, string password)
        {
            if (username == "luffy@gmail.com" && password == "Rumah123!")
            {
                // Successful login
                List<Voucher> voucherList = await _context.Voucher.ToListAsync();

                return View(voucherList);// Redirect to AdminHome action
            }
            else
            {
                // Failed login
                ViewBag.ErrorMessage = "Invalid username or password";
                return View("Index"); // Replace "YourLoginView" with your actual login view name
            }
            //return View();
        }

        public async Task<IActionResult> SalesInfo()
        {
            List<HistoryView> transactions = await _context.Transactions
                .Join(
                    _context.Voucher,
                    transaction => transaction.VoucherID,
                    voucher => voucher.VoucherID,
                    (transaction, voucher) => new { transaction, voucher })
                .Join(
                    _context.Customer,
                    joined => joined.transaction.CustomerID,
                    customer => customer.CustomerID,
                    (joined, customer) => new HistoryView
                    {
                        TransactionID = joined.transaction.TransactionID,
                        CustomerID = joined.transaction.CustomerID,
                        VoucherID = joined.transaction.VoucherID,
                        GiftcardCode = joined.transaction.GiftcardCode,
                        VoucherName = joined.voucher.VoucherName,
                        VoucherValue = joined.voucher.VoucherValue,
                        VoucherPrice = joined.voucher.VoucherPrice,
                        CustomerName = customer.CustomerName
                    })
                .ToListAsync();

            ViewData["SalesCount"] = transactions.Count;

            return View(transactions);
        }
    }
}
