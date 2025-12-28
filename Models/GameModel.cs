namespace TicTacToe.Models
{
    public class GameModel
    {
        public char[,] Board { get; set; }
        public char CurrentPlayer { get; set; }
        public string Message { get; set; }

        public GameModel()
        {
            Board = new char[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Board[i, j] = ' ';
            CurrentPlayer = 'X';
            Message = "Player X's turn";
        }

        public bool MakeMove(int row, int col)
        {
            if (Board[row, col] == ' ')
            {
                Board[row, col] = CurrentPlayer;
                if (CheckWin(CurrentPlayer))
                {
                    Message = $"Player {CurrentPlayer} wins!";
                    return true;
                }
                if (IsBoardFull())
                {
                    Message = "It's a draw!";
                    return true;
                }
                CurrentPlayer = CurrentPlayer == 'X' ? 'O' : 'X';
                Message = $"Player {CurrentPlayer}'s turn";
                return true;
            }
            return false;
        }

        private bool CheckWin(char player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Board[i, 0] == player && Board[i, 1] == player && Board[i, 2] == player)
                    return true;
                if (Board[0, i] == player && Board[1, i] == player && Board[2, i] == player)
                    return true;
            }
            if (Board[0, 0] == player && Board[1, 1] == player && Board[2, 2] == player)
                return true;
            if (Board[0, 2] == player && Board[1, 1] == player && Board[2, 0] == player)
                return true;
            return false;
        }

        private bool IsBoardFull()
        {
            foreach (char cell in Board)
                if (cell == ' ')
                    return false;
            return true;
        }
    }
}

