using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Minesweeper.Identity.Data
{
    public class MinesweeperIdentityDbContext : IdentityDbContext
    {
        public MinesweeperIdentityDbContext(DbContextOptions<MinesweeperIdentityDbContext> options)
            : base(options)
        {
        }
    }
}