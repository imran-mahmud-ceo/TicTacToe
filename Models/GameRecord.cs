using System;
using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Models
{
    public class GameRecord
    {
        public int Id { get; set; }

        public string BoardState { get; set; } = "---------";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? UserId { get; set; }

        public string CurrentPlayer { get; set; } = "X";

        public string Message { get; set; } = "Game in progress";

        // --- FIX IS HERE ---
        // Changed from a calculated property to a stored property.
        // This allows the Controller to save "X", "O", or "Draw" into the database.
        public string? Winner { get; set; }
    }
}