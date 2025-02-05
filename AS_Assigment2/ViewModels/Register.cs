using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS_Assigment2.ViewModels
{
    public class Register
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Credit Card Number is required.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Credit Card Number must be exactly 16 digits.")]
        public string CreditCardNo { get; set; }


        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        [RegularExpression(@"^\d{8,15}$", ErrorMessage = "Invalid mobile number format.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Delivery Address is required.")]
        [StringLength(255, ErrorMessage = "Delivery Address cannot exceed 255 characters.")]
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "About Me is required.")]
        [DataType(DataType.MultilineText)]
        public string AboutMe { get; set; }
    }
}
