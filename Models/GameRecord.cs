using System;
using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Models
{
    // This class defines what our SQL Database table looks like
    public class GameRecord
    {
        [Key]
        public int Id { get; set; }           // Primary Key
        public string BoardState { get; set; } // Stores board like "XOX--O---"
        public char CurrentPlayer { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}