using Microsoft.AspNetCore.Identity;

namespace Minesweeper.Identity.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}