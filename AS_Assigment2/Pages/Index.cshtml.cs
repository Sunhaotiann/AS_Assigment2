using AS_Assigment2.Model;
using AS_Assigment2.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using AS_Assigment2.ViewModels;

namespace AS_Assigment2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(AuthDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<IndexModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public Register LoggedInUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            // Check if session exists
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                _logger.LogInformation("Session expired. Logging out user.");
                await _signInManager.SignOutAsync(); // Log out the user
                return RedirectToPage("/Login");
            }

            // Retrieve the last activity time
            var lastActivity = HttpContext.Session.GetString("LastActivityTime");
            if (lastActivity != null && DateTime.TryParse(lastActivity, out DateTime lastActivityTime))
            {
                var sessionTimeout = TimeSpan.FromSeconds(10); // Set session timeout duration

                if (DateTime.Now - lastActivityTime > sessionTimeout)
                {
                    _logger.LogInformation("Session timed out. Logging out user.");
                    HttpContext.Session.Clear(); // Clear session data
                    await _signInManager.SignOutAsync(); // Log out the user
                    return RedirectToPage("/Login");
                }
            }

            // Update the session timestamp to keep the session active
            HttpContext.Session.SetString("LastActivityTime", DateTime.Now.ToString());

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var encryptedUser = _context.Registers.FirstOrDefault(r => r.Email == user.Email);
                if (encryptedUser != null)
                {
                    // Decrypt encrypted fields
                    LoggedInUser = new Register
                    {
                        FullName = encryptedUser.FullName,
                        CreditCardNo = EncryptionHelper.Decrypt(encryptedUser.CreditCardNo),
                        Gender = encryptedUser.Gender,
                        MobileNo = EncryptionHelper.Decrypt(encryptedUser.MobileNo),
                        DeliveryAddress = EncryptionHelper.Decrypt(encryptedUser.DeliveryAddress),
                        Email = encryptedUser.Email,
                        AboutMe = encryptedUser.AboutMe
                    };
                }
            }

            return Page();
        }
    }
}
