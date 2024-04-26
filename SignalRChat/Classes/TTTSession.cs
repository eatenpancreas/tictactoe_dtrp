using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRChat.Classes
{
    public class TTTSession
    {
        public string name { get; set; }
        public string player_1 { get; set; }
        public string? player_2 { get; set; }
        public string? currentPlayer { get; set; }

        private string[] board = new string[9];

        public TTTSession(string name, string owner)
        {
            this.name = name;
            this.player_1 = owner;
            this.currentPlayer = owner;
            Array.Fill(board, "");
        }

        public void addOpponent(string opponent)
        {
            this.player_2 = opponent;
        }

        public bool move(int position, string player)
        {
            if (player != currentPlayer || position < 0 || position >= 9 || !string.IsNullOrEmpty(board[position]))
            {
                return false;
            }

            board[position] = player;
            currentPlayer = (player == player_1) ? player_2 : player_1; 
            return true;
        }

        public string checkWinner()
        {
            int[,] winConditions = new int[,]
            {
                {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
                {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
                {0, 4, 8}, {2, 4, 6}
            };

            for (int i = 0; i < 8; i++)
            {
                int a = winConditions[i, 0], b = winConditions[i, 1], c = winConditions[i, 2];
                if (!string.IsNullOrEmpty(board[a]) && board[a] == board[b] && board[b] == board[c])
                {
                    return board[a]; 
                }
            }

            return board.All(x => !string.IsNullOrEmpty(x)) ? "Draw" : "";
        }

        public string[] getBoard()
        {
            return board;
        }
    }
}
