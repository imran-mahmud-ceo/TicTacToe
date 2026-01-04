using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicTacToe.Data;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly TicTacToeContext _context;

        public GameController(TicTacToeContext context)
        {
            _context = context;
        }

        // --- 1. VIEW GAME ---
        public IActionResult Index(int? id, string difficulty)
        {
            if (id == null) return RedirectToAction("CreateGame");

            var game = _context.GameRecords.FirstOrDefault(g => g.Id == id);
            if (game == null) return NotFound();

            if (string.IsNullOrEmpty(difficulty))
            {
                difficulty = TempData["Difficulty"]?.ToString() ?? "Human";
            }
            ViewBag.Difficulty = difficulty;
            TempData["Difficulty"] = difficulty;

            return View("Game", game);
        }

        // --- 2. MENU SCREEN ---
        public IActionResult VsComputer()
        {
            return View();
        }

        // --- 3. CREATE GAME ---
        public async Task<IActionResult> CreateGame(string difficulty = "Human")
        {
            var newGame = new GameRecord
            {
                BoardState = "---------",
                CreatedAt = DateTime.Now,
                CurrentPlayer = "X",
                Message = "Your Turn (X)",
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Winner = null
            };

            _context.GameRecords.Add(newGame);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = newGame.Id, difficulty = difficulty });
        }

        // --- 4. HUMAN MOVE ---
        [HttpPost]
        public async Task<IActionResult> PlayMove(int id, int row, int col, string difficulty)
        {
            var game = await _context.GameRecords.FindAsync(id);
            if (game == null) return NotFound();

            // Restore difficulty
            if (string.IsNullOrEmpty(difficulty)) difficulty = "Human";
            TempData["Difficulty"] = difficulty;

            if (game.Winner != null) return RedirectToAction("Index", new { id = game.Id, difficulty });

            // A. Check if Spot is Taken
            int index = (row * 3) + col;
            char[] board = game.BoardState.ToCharArray();
            if (board[index] != '-') return RedirectToAction("Index", new { id, difficulty });

            // B. Identify Player
            // If vs Computer, Human is ALWAYS X. If Human vs Human, rely on CurrentPlayer.
            char markToPlace = char.Parse(game.CurrentPlayer);

            // Security: Prevent Human from playing O in Computer Mode
            if (difficulty != "Human" && markToPlace == 'O')
            {
                return RedirectToAction("Index", new { id, difficulty });
            }

            // C. Place Move & Switch Turn
            board[index] = markToPlace;
            game.BoardState = new string(board);

            // Switch Turn
            game.CurrentPlayer = (markToPlace == 'X') ? "O" : "X";

            CheckWinner(game, difficulty);
            await _context.SaveChangesAsync();

            // STOP! We do NOT run the computer move here anymore.
            // We return to the View so the user sees "Computer is thinking..."
            return RedirectToAction("Index", new { id = game.Id, difficulty });
        }

        // --- 5. COMPUTER MOVE (Triggered by JavaScript) ---
        [HttpPost]
        public async Task<IActionResult> ComputerMove(int id, string difficulty)
        {
            var game = await _context.GameRecords.FindAsync(id);
            if (game == null) return NotFound();

            // Restore difficulty
            if (string.IsNullOrEmpty(difficulty)) difficulty = "Human";
            TempData["Difficulty"] = difficulty;

            // Only run if it is actually O's turn and game isn't over
            if (game.CurrentPlayer == "O" && game.Winner == null && difficulty != "Human")
            {
                // Optional Server Delay (for realism)
                await Task.Delay(500);

                int aiIndex = TicTacToeAI.GetBestMove(game.BoardState, difficulty);
                if (aiIndex != -1)
                {
                    char[] aiBoard = game.BoardState.ToCharArray();
                    aiBoard[aiIndex] = 'O';
                    game.BoardState = new string(aiBoard);

                    // Switch back to Human
                    game.CurrentPlayer = "X";

                    CheckWinner(game, difficulty);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", new { id = game.Id, difficulty });
        }

        // --- HELPER: CHECK WINNER ---
        private void CheckWinner(GameRecord game, string difficulty)
        {
            char[] board = game.BoardState.ToCharArray();
            int[][] wins = new int[][]
            {
                new int[] {0,1,2}, new int[] {3,4,5}, new int[] {6,7,8},
                new int[] {0,3,6}, new int[] {1,4,7}, new int[] {2,5,8},
                new int[] {0,4,8}, new int[] {2,4,6}
            };

            // 1. Check Win
            foreach (var w in wins)
            {
                if (board[w[0]] != '-' && board[w[0]] == board[w[1]] && board[w[1]] == board[w[2]])
                {
                    game.Winner = board[w[0]].ToString();

                    if (game.Winner == "X")
                        game.Message = "You Win! 🎉";
                    else
                        game.Message = (difficulty == "Human") ? "Player 2 Wins! 👤" : "Computer Wins! 🤖";

                    return;
                }
            }

            // 2. Check Draw
            if (!game.BoardState.Contains("-"))
            {
                game.Winner = "Draw";
                game.Message = "It's a Draw! 🤝";
            }
            else
            {
                // 3. Update Status Message for Next Turn
                if (game.CurrentPlayer == "X")
                    game.Message = "Your Turn (X)";
                else
                {
                    if (difficulty == "Human")
                        game.Message = "Player 2's Turn (O)";
                    else
                        game.Message = "Computer is thinking... 🤖";
                }
            }
        }
    }
}