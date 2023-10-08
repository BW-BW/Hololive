using System.ComponentModel.DataAnnotations;

namespace Hololive.Models
{
    public class Transactions
    {
        [Key]//primary key
        public int TransactionID { get; set; }

        
        [Display(Name = "Customer ID: ")]
        public int CustomerID { get; set; }

        
        [Display(Name = "Voucher ID: ")]
        public int VoucherID { get; set; }

        
        [Display(Name = "Giftcard Code: ")]
        public string GiftcardCode { get; set; }
    }
}
