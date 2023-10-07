using System.ComponentModel.DataAnnotations;

namespace Hololive.Models
{
    public class Voucher
    {
        [Key]//primary key
        public int VoucherID { get; set; }

        [Required]
        [Display(Name = "Voucher Name: ")]
        public string VoucherName { get; set; }

        [Required]
        [Display(Name = "Voucher Value: ")]
        public string VoucherValue { get; set; }

        [Required]
        [Display(Name = "Voucher Price: ")]
        public decimal VoucherPrice { get; set; }

        
        [Display(Name = "Voucher Link: ")]
        public string VoucherLink { get; set; }

        public string VoucherS3Key { get; set; }
    }
}
