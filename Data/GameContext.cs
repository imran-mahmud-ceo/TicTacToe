using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // <--- Added this
using Microsoft.EntityFrameworkCore;
using TicTacToe.Models;

namespace TicTacToe.Data
{
    // Inherit from IdentityDbContext to get User tables automatically
    public class GameContext : IdentityDbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options)
        {
        }

        // Your existing table for game history
        public DbSet<GameRecord> Games { get; set; }
    }
}