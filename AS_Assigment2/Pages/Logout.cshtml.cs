using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AS_Assigment2.Model;

namespace AS_Assigment2.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly AuthDbContext _context;

        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger, AuthDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // Log user activity before logging out
            var email = HttpContext.Session.GetString("UserEmail");
            if (!string.IsNullOrEmpty(email))
            {
                LogUserActivity(email, "User Logged Out");
            }

            // Clear session data
            HttpContext.Session.Clear();

            // Perform logout
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            return RedirectToPage("/Login");
        }

        private void LogUserActivity(string email, string activity)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                Email = email,
                Activity = activity,
                Timestamp = System.DateTime.UtcNow
            });
            _context.SaveChanges();
        }
    }
}
