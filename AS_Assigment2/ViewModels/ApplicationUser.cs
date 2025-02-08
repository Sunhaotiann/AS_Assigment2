using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace AS_Assigment2.ViewModels
{
    public class ApplicationUser : IdentityUser
    {
        public List<string> PasswordHistory { get; set; } = new List<string>();
    }
}
