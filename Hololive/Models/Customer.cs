using System.ComponentModel.DataAnnotations;

namespace Hololive.Models
{
    public class Customer
    {
        [Key]//primary key
        public int CustomerID { get; set; }

        [Required]
        [Display(Name = "Customer Name: ")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Email: ")]
        public string CustomerEmail { get; set; }

        [Required]
        [Display(Name = "Password: ")]
        public string CustomerPassword { get; set; }

        [Required]
        [Display(Name = "Phone Number: ")]
        public int CustomerPhone { get; set; }
    }
}
