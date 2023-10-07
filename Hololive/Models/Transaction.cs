using System.ComponentModel.DataAnnotations;

namespace Hololive.Models
{
    public class Transaction
    {
        [Key]//primary key
        public int TransactionID { get; set; }

        [Required]
        [Display(Name = "Customer Name: ")]
        public string CustomerID { get; set; }
    }
}
