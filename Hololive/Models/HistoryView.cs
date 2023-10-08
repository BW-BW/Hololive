using System.ComponentModel.DataAnnotations;

namespace Hololive.Models
{
    public class HistoryView
    {
        [Key]//primary key
        public int TransactionID { get; set; }


        [Display(Name = "Customer ID: ")]
        public int CustomerID { get; set; }


        [Display(Name = "Voucher ID: ")]
        public int VoucherID { get; set; }


        [Display(Name = "Giftcard Code: ")]
        public string GiftcardCode { get; set; }

        [Display(Name = "Customer Name: ")]
        public string CustomerName { get; set; }

        [Display(Name = "Voucher Name: ")]
        public string VoucherName { get; set; }

        
        [Display(Name = "Voucher Value: ")]
        public string VoucherValue { get; set; }

        
        [Display(Name = "Voucher Price: ")]
        public decimal VoucherPrice { get; set; }
    }
}
