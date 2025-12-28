using Microsoft.AspNetCore.Mvc;
using TicTacToe.Models;
using TicTacToe.Data;
using System.Linq;
using System;

namespace TicTacToe.Controllers
{
    public class GameController : Controller
    {
        private readonly GameContext _context;

        public GameController(GameContext context)
        {
            _context = context;
        }

        // 1. LIST GAMES (History)
        // 1. LIST GAMES (History - Finished Only)
        public IActionResult Index()
        {
            // Filter: Only show games where the message does NOT contain the word "turn".
            // This hides "Player X's turn" (active games) and shows only "Wins" or "Draws".
            var finishedGames = _context.Games
                .Where(g => !g.Message.Contains("turn"))
                .OrderByDescending(g => g.CreatedAt)
                .ToList();

            return View(finishedGames);
        }

        // 2. CREATE NEW GAME
        public IActionResult CreateGame()
        {
            var newGame = new GameRecord
            {
                BoardState = "         ", // 9 spaces
                CurrentPlayer = 'X',
                Message = "Player X's turn",
                CreatedAt = DateTime.Now
            };

            _context.Games.Add(newGame);
            _context.SaveChanges();

            return RedirectToAction("Play", new { id = newGame.Id });
        }

        // 3. PLAY GAME (The Board)
        public IActionResult Play(int id)
        {
            var gameRecord = _context.Games.Find(id);
            if (gameRecord == null) return NotFound();

            var model = new GameModel
            {
                CurrentPlayer = gameRecord.CurrentPlayer,
                Message = gameRecord.Message,
                Board = StringToBoard(gameRecord.BoardState)
            };

            ViewBag.GameId = gameRecord.Id;
            return View("Game", model);
        }

        // 4. MAKE MOVE (Update Game)
        [HttpPost]
        public IActionResult MakeMove(int id, int row, int col)
        {
            var gameRecord = _context.Games.Find(id);
            if (gameRecord == null) return NotFound();

            var model = new GameModel
            {
                Board = StringToBoard(gameRecord.BoardState),
                CurrentPlayer = gameRecord.CurrentPlayer,
                Message = gameRecord.Message
            };

            if (model.MakeMove(row, col))
            {
                gameRecord.BoardState = BoardToString(model.Board);
                gameRecord.CurrentPlayer = model.CurrentPlayer;
                gameRecord.Message = model.Message;

                _context.Games.Update(gameRecord);
                _context.SaveChanges();
            }

            return RedirectToAction("Play", new { id = id });
        }

        // 5. DELETE GAME
        public IActionResult Delete(int id)
        {
            var game = _context.Games.Find(id);
            if (game != null)
            {
                _context.Games.Remove(game);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // HELPERS
        private string BoardToString(char[,] board)
        {
            char[] flat = new char[9];
            int k = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    flat[k++] = board[i, j];
            return new string(flat);
        }

        private char[,] StringToBoard(string str)
        {
            char[,] board = new char[3, 3];
            int k = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    board[i, j] = str[k++];
            return board;
        }
    }
}