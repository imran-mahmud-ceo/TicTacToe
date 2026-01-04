using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Services
{
    public static class TicTacToeAI
    {
        // --- PUBLIC API ---
        public static int GetBestMove(string boardState, string difficulty)
        {
            char[] board = boardState.ToCharArray();
            List<int> availableMoves = GetAvailableMoves(board);

            if (availableMoves.Count == 0) return -1;

            // 1. EASY: Pure Random
            if (difficulty == "Easy")
            {
                Random rand = new Random();
                return availableMoves[rand.Next(availableMoves.Count)];
            }

            // 2. MEDIUM: Win if can, Block if must, otherwise Random
            if (difficulty == "Medium")
            {
                // Try to win
                int winMove = FindWinningMove(board, 'O');
                if (winMove != -1) return winMove;

                // Try to block human
                int blockMove = FindWinningMove(board, 'X');
                if (blockMove != -1) return blockMove;

                // Otherwise random
                Random rand = new Random();
                return availableMoves[rand.Next(availableMoves.Count)];
            }

            // 3. HARD: Unbeatable Minimax
            if (difficulty == "Hard")
            {
                var bestMove = Minimax(board, 'O');
                return bestMove.index;
            }

            return availableMoves[0]; // Fallback
        }

        // --- PRIVATE HELPERS ---

        private static List<int> GetAvailableMoves(char[] board)
        {
            List<int> moves = new List<int>();
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == '-') moves.Add(i);
            }
            return moves;
        }

        private static int FindWinningMove(char[] board, char player)
        {
            // Simple check: try every empty spot, see if it wins
            var moves = GetAvailableMoves(board);
            foreach (var index in moves)
            {
                board[index] = player;
                if (CheckWin(board, player))
                {
                    board[index] = '-'; // Undo
                    return index;
                }
                board[index] = '-'; // Undo
            }
            return -1;
        }

        // The Recursive Minimax Function
        private static (int index, int score) Minimax(char[] board, char player)
        {
            var availMoves = GetAvailableMoves(board);

            // Base Checks
            if (CheckWin(board, 'O')) return (-1, 10);
            if (CheckWin(board, 'X')) return (-1, -10);
            if (availMoves.Count == 0) return (-1, 0);

            List<(int index, int score)> moves = new List<(int index, int score)>();

            foreach (var idx in availMoves)
            {
                board[idx] = player; // Make move

                int score;
                if (player == 'O') // AI
                {
                    score = Minimax(board, 'X').score;
                }
                else // Human
                {
                    score = Minimax(board, 'O').score;
                }

                board[idx] = '-'; // Undo move
                moves.Add((idx, score));
            }

            // If it's AI's turn ('O'), we want the MAX score
            if (player == 'O')
            {
                return moves.OrderByDescending(x => x.score).First();
            }
            // If it's Human's turn ('X'), they will pick the MIN score (best for them, worst for us)
            else
            {
                return moves.OrderBy(x => x.score).First();
            }
        }

        private static bool CheckWin(char[] board, char p)
        {
            int[][] wins = {
                new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8}, // Rows
                new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8}, // Cols
                new[]{0,4,8}, new[]{2,4,6}                // Diags
            };
            return wins.Any(w => board[w[0]] == p && board[w[1]] == p && board[w[2]] == p);
        }
    }
}