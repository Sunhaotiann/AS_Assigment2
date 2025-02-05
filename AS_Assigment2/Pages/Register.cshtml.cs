using AS_Assigment2.ViewModels;
using AS_Assigment2.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AS_Assigment2.Helpers;
using System.Web;

namespace AS_Assigment2.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthDbContext _context;

        public RegisterModel(UserManager<IdentityUser> userManager, AuthDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public Register Register { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //  Sanitize Inputs to prevent XSS
            Register.FullName = HttpUtility.HtmlEncode(Register.FullName);
            Register.Email = HttpUtility.HtmlEncode(Register.Email);
            Register.AboutMe = HttpUtility.HtmlEncode(Register.AboutMe);
            Register.MobileNo = HttpUtility.HtmlEncode(Register.MobileNo);
            Register.CreditCardNo = HttpUtility.HtmlEncode(Register.CreditCardNo);

            
            if (!Regex.IsMatch(Register.FullName, @"^[a-zA-Z\s]+$"))
            {
                ModelState.AddModelError("Register.FullName", "Full Name can only contain letters and spaxces.");
                return Page();
            }

            
            if (!Regex.IsMatch(Register.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("Register.Email", "Invalid email format.");
                return Page();
            }

            
            if (!Regex.IsMatch(Register.MobileNo, @"^\d{8,15}$"))
            {
                ModelState.AddModelError("Register.MobileNo", "Mobile number must be 8 to 15 digits long and contain only numbers.");
                return Page();
            }

            
            if (!Regex.IsMatch(Register.CreditCardNo, @"^\d{16}$"))
            {
                ModelState.AddModelError("Register.CreditCardNo", "Credit Card Number must be exactly 16 digits long.");
                return Page();
            }

           
            string passwordStrength = GetPasswordStrength(Register.Password);
            if (passwordStrength == "Weak")
            {
                ModelState.AddModelError("Register.Password", "Your password is too weak. Try adding more characters, numbers, uppercase, and special symbols.");
                return Page();
            }

           
            var existingUser = await _userManager.FindByEmailAsync(Register.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Register.Email", "Email already exists.");
                return Page();
            }

           
            var identityUser = new IdentityUser
            {
                UserName = Register.Email,
                Email = Register.Email
            };

            var result = await _userManager.CreateAsync(identityUser, Register.Password);

            if (result.Succeeded)
            {
                var registerUser = new Register
                {
                    FullName = Register.FullName,
                    CreditCardNo = EncryptionHelper.Encrypt(Register.CreditCardNo), // Encrypt Credit Card
                    Gender = Register.Gender,
                    MobileNo = EncryptionHelper.Encrypt(Register.MobileNo), // Encrypt Mobile Number
                    DeliveryAddress = EncryptionHelper.Encrypt(Register.DeliveryAddress), // Encrypt Address
                    Email = Register.Email,
                    Password = _userManager.PasswordHasher.HashPassword(identityUser, Register.Password), // Hash Password
                    AboutMe = Register.AboutMe
                };

                _context.Registers.Add(registerUser);
                await _context.SaveChangesAsync();

                return RedirectToPage("Success");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private string GetPasswordStrength(string password)
        {
            int score = 0;

            if (password.Length >= 12) score++;
            if (Regex.IsMatch(password, @"[A-Z]")) score++;
            if (Regex.IsMatch(password, @"[a-z]")) score++;
            if (Regex.IsMatch(password, @"\d")) score++;
            if (Regex.IsMatch(password, @"[\W_]")) score++;

            return score switch
            {
                >= 5 => "Strong",
                >= 3 => "Moderate",
                _ => "Weak"
            };
        }
    }
}
