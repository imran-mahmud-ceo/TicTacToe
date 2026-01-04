using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using TicTacToe.Data;
using TicTacToe.Models;

namespace TicTacToe.Controllers
{
    public class HomeController : Controller
    {
        private readonly TicTacToeContext _context;

        public HomeController(TicTacToeContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // If user is NOT logged in, just show the landing page
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            // If user IS logged in, find THEIR games
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myGames = _context.GameRecords
                            .Where(g => g.UserId == userId)
                            .OrderByDescending(g => g.CreatedAt) // Show newest first
                            .ToList();

            // Send these games to the Dashboard view
            return View("Dashboard", myGames);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        // OPEN: Controllers/HomeController.cs
        // Add this method anywhere inside the class

        [HttpPost] // We use POST for security so people can't delete via a link
        [ValidateAntiForgeryToken]
        public IActionResult DeleteGame(int id)
        {
            // 1. Find the game
            var game = _context.GameRecords.Find(id);

            // 2. Check if game exists and belongs to the current user (Security Check)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (game != null && game.UserId == currentUserId)
            {
                _context.GameRecords.Remove(game);
                _context.SaveChanges();
            }

            // 3. Reload the Dashboard
            return RedirectToAction("Index");
        }
    }
}
