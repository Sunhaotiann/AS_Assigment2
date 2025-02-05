using AS_Assigment2.ViewModels;
using AS_Assigment2.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Web;

namespace AS_Assigment2.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly AuthDbContext _context;

        [TempData]
        public string SuccessMessage { get; set; }

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger,
            AuthDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed.");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Sanitize Inputs to prevent XSS
            LModel.Email = HttpUtility.HtmlEncode(LModel.Email);
            LModel.Password = HttpUtility.HtmlEncode(LModel.Password);

            var user = await _userManager.FindByEmailAsync(LModel.Email);
            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", LModel.Email);
                ModelState.AddModelError("", "Invalid email or password.");
                return Page();
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError("", "Your account is locked due to multiple failed attempts. Try again later.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, LModel.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in successfully: {Email}", LModel.Email);

                await _userManager.ResetAccessFailedCountAsync(user);

                // Store user info securely in session
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserId", user.Id);
                HttpContext.Session.SetString("IsAuthenticated", "true");

                LogUserActivity(user.Email, "Login Successful");

                TempData["SuccessMessage"] = "Login successful! Welcome back.";
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            LogUserActivity(LModel.Email, "Failed Login Attempt");

            return Page();
        }

        private void LogUserActivity(string email, string activity)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                Email = email,
                Activity = activity,
                Timestamp = DateTime.UtcNow
            });
            _context.SaveChanges();
        }
    }
}
