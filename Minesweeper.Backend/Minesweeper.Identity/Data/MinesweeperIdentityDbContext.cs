using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Minesweeper.Identity.Data.Entities;

namespace Minesweeper.Identity.Data
{
    public class MinesweeperIdentityDbContext : IdentityDbContext<AppUser>
    {
        public MinesweeperIdentityDbContext(DbContextOptions<MinesweeperIdentityDbContext> options)
            : base(options)
        {
        }
    }
}