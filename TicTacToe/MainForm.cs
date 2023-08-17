using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class MainForm : Form
    {
        private bool isPlayerX;
        private const int buttonSize = 90;
        private const int padding = 10;
        private const int gridSize = 3;

        private List<Button> buttons;

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            int formWidth = 330;
            int formHeight = 350;

            Size = new Size(formWidth, formHeight);
            Text = "Крестики-нолики";

            buttons = new List<Button>();

            for (int i = 0; i < gridSize * gridSize; i++)
            {
                Button button = new Button();
                button.Size = new Size(ButtonSize, ButtonSize);
                button.Font = new Font("Arial", 36);
                button.Tag = i;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = Color.Black;
                button.Click += Button_Click;

                int row = i / gridSize;
                int col = i % gridSize;
                button.Location = new Point(padding + col * (ButtonSize + padding), padding + row * (ButtonSize + padding));

                buttons.Add(button);

                Controls.Add(button);
            }

            isPlayerX = true;
            IsComputerTurn = false;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (button.Text != "")
            {
                return;
            }

            if (isPlayerX)
            {
                button.Text = "X";
            }
            else
            {
                button.Text = "O";
            }

            button.Enabled = false;
            isPlayerX = !isPlayerX;

            if (CheckForWin("X"))
            {
                MessageBox.Show("Победил игрок X!", "Победа");
                ResetGame();
            }
            else if (isPlayerX)
            {
                // Пользователь совершает второй ход
            }
            else
            {
                MakeComputerMove();
                if (CheckForWin("O"))
                {
                    MessageBox.Show("Победил компьютер!", "Победа");
                    ResetGame();
                }
            }
        }

        private Dictionary<string, int> moveScores = new Dictionary<string, int>();

        public bool IsComputerTurn { get => IsComputerTurn1; set => IsComputerTurn1 = value; }
        public bool IsComputerTurn1 { get => IsComputerTurn2; set => IsComputerTurn2 = value; }
        public bool IsComputerTurn2 { get; set; }

        public static int ButtonSize => buttonSize;

        private void MakeComputerMove()
        {
            // Проверка, есть ли какой-нибудь выигрышный ход для игрока
            Button winningMove = GetWinningMove("X");

            // Если для игрока есть выигрышный ход, заблокировать его
            if (winningMove != null)
            {
                winningMove.Text = "O";
                winningMove.Enabled = false;
            }
            else
            {
                // Проверка, сделал ли игрок какой-либо ход
                bool playerHasMoved = false;

                foreach (Button button in buttons)
                {
                    if (button.Text == "X")
                    {
                        playerHasMoved = true;
                        break;
                    }
                }

                // If the player has made a move, play strategically
                if (playerHasMoved)
                {
                    // Check if there is any winning move for the computer
                    Button winningMoveForComputer = GetWinningMove("O");

                    // If there is a winning move for the computer, play it
                    if (winningMoveForComputer != null)
                    {
                        winningMoveForComputer.Text = "O";
                        winningMoveForComputer.Enabled = false;
                    }
                    else
                    {
                        // Otherwise, play a random available move with consideration of move scores
                        List<Button> availableButtons = new List<Button>();

                        foreach (Button button in buttons)
                        {
                            if (button.Text == "")
                            {
                                availableButtons.Add(button);
                            }
                        }

                        if (availableButtons.Count > 0)
                        {
                            int maxScore = int.MinValue;
                            List<Button> bestMoves = new List<Button>();

                            foreach (Button button in availableButtons)
                            {
                                int score = GetMoveScore(button);

                                if (score > maxScore)
                                {
                                    maxScore = score;
                                    bestMoves.Clear();
                                    bestMoves.Add(button);
                                }
                                else if (score == maxScore)
                                {
                                    bestMoves.Add(button);
                                }
                            }

                            Random random = new Random();
                            int randomIndex = random.Next(0, bestMoves.Count);

                            Button selectedMove = bestMoves[randomIndex];
                            selectedMove.Text = "O";
                            selectedMove.Enabled = false;

                            UpdateMoveScore(selectedMove);
                        }
                    }
                }
                else
                {
                    // If the player hasn't made a move yet, play in the top-left cell
                    buttons[0].Text = "O";
                    buttons[0].Enabled = false;
                }
            }

            if (IsBoardFull() && !CheckForWin("X") && !CheckForWin("O"))
            {
                MessageBox.Show("Ничья!");
                ResetGame();
            }

            isPlayerX = true;
        }

        private int GetMoveScore(Button button)
        {
            int score = 0;

            if (moveScores.ContainsKey(button.Name))
            {
                score = moveScores[button.Name];
            }

            return score;
        }

        private void UpdateMoveScore(Button button)
        {
            if (moveScores.ContainsKey(button.Name))
            {
                moveScores[button.Name]++;
            }
            else
            {
                moveScores[button.Name] = 1;
            }
        }

        private Button GetWinningMove(string symbol)
        {
            // Check rows
            for (int row = 0; row < 3; row++)
            {
                int count = 0;
                Button emptyButton = null;

                for (int col = 0; col < 3; col++)
                {
                    Button button = buttons[row * 3 + col];
                    if (button.Text == symbol)
                    {
                        count++;
                    }
                    else if (button.Text == "")
                    {
                        emptyButton = button;
                    }
                }

                if (count == 2 && emptyButton != null)
                {
                    return emptyButton;
                }
            }

            // Check columns
            for (int col = 0; col < 3; col++)
            {
                int count = 0;
                Button emptyButton = null;

                for (int row = 0; row < 3; row++)
                {
                    Button button = buttons[row * 3 + col];
                    if (button.Text == symbol)
                    {
                        count++;
                    }
                    else if (button.Text == "")
                    {
                        emptyButton = button;
                    }
                }

                if (count == 2 && emptyButton != null)
                {
                    return emptyButton;
                }
            }

            // Check diagonals
            if (buttons[0].Text == symbol && buttons[4].Text == symbol && buttons[8].Text == "")
            {
                return buttons[8];
            }
            if (buttons[0].Text == symbol && buttons[8].Text == symbol && buttons[4].Text == "")
            {
                return buttons[4];
            }
            if (buttons[4].Text == symbol && buttons[8].Text == symbol && buttons[0].Text == "")
            {
                return buttons[0];
            }
            if (buttons[2].Text == symbol && buttons[4].Text == symbol && buttons[6].Text == "")
            {
                return buttons[6];
            }
            if (buttons[2].Text == symbol && buttons[6].Text == symbol && buttons[4].Text == "")
            {
                return buttons[4];
            }
            if (buttons[4].Text == symbol && buttons[6].Text == symbol && buttons[2].Text == "")
            {
                return buttons[2];
            }

            return null;
        }

        private int MiniMax(Button button, int depth, bool isMaximizingPlayer, int alpha, int beta)
        {
            string opponentSymbol = isMaximizingPlayer ? "X" : "O";
            string currentPlayerSymbol = isMaximizingPlayer ? "O" : "X";

            if (CheckForWin(opponentSymbol))
            {
                return -10 + depth;
            }
            else if (CheckForWin(currentPlayerSymbol))
            {
                return 10 - depth;
            }
            else if (IsBoardFull())
            {
                return 0;
            }

            if (isMaximizingPlayer)
            {
                int bestScore = int.MinValue;

                foreach (Button b in buttons)
                {
                    if (b.Text == "")
                    {
                        b.Text = currentPlayerSymbol;
                        int score = MiniMax(b, depth + 1, false, alpha, beta);
                        b.Text = "";

                        bestScore = Math.Max(score, bestScore);
                        alpha = Math.Max(alpha, bestScore);

                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                foreach (Button b in buttons)
                {
                    if (b.Text == "")
                    {
                        b.Text = currentPlayerSymbol;
                        int score = MiniMax(b, depth + 1, true, alpha, beta);
                        b.Text = "";

                        bestScore = Math.Min(score, bestScore);
                        beta = Math.Min(beta, bestScore);

                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

                return bestScore;
            }
        }

        private int AlphaBeta(int depth, int alpha, int beta, bool isMaximizingPlayer)
        {
            string opponentSymbol = isMaximizingPlayer ? "X" : "O";
            string currentPlayerSymbol = isMaximizingPlayer ? "O" : "X";

            if (CheckForWin(opponentSymbol))
            {
                return -10 + depth;
            }
            else if (CheckForWin(currentPlayerSymbol))
            {
                return 10 - depth;
            }
            else if (IsBoardFull())
            {
                return 0;
            }

            if (isMaximizingPlayer)
            {
                int bestScore = int.MinValue;

                foreach (Button button in buttons)
                {
                    if (button.Text == "")
                    {
                        button.Text = currentPlayerSymbol;
                        int score = AlphaBeta(depth + 1, alpha, beta, false);
                        button.Text = "";

                        bestScore = Math.Max(score, bestScore);
                        alpha = Math.Max(alpha, bestScore);

                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                foreach (Button button in buttons)
                {
                    if (button.Text == "")
                    {
                        button.Text = currentPlayerSymbol;
                        int score = AlphaBeta(depth + 1, alpha, beta, true);
                        button.Text = "";

                        bestScore = Math.Min(score, bestScore);
                        beta = Math.Min(beta, bestScore);

                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

                return bestScore;
            }
        }

        private bool IsBoardFull()
        {
            foreach (Button button in buttons)
            {
                if (button.Text == "")
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckForWin(string symbol)
        {
            // Проверка по строкам
            for (int row = 0; row < gridSize; row++)
            {
                if (buttons[row * gridSize].Text == symbol &&
                    buttons[row * gridSize + 1].Text == symbol &&
                    buttons[row * gridSize + 2].Text == symbol)
                {
                    return true;
                }
            }

            // Проверка по столбцам
            for (int col = 0; col < gridSize; col++)
            {
                if (buttons[col].Text == symbol &&
                    buttons[col + gridSize].Text == symbol &&
                    buttons[col + gridSize * 2].Text == symbol)
                {
                    return true;
                }
            }

            // Проверка по диагонали (левая верхняя до правая нижняя)
            if (buttons[0].Text == symbol &&
                buttons[gridSize + 1].Text == symbol &&
                buttons[2 * gridSize + 2].Text == symbol)
            {
                return true;
            }

            // Проверка по диагонали (правая верхняя до левая нижняя)
            if (buttons[2].Text == symbol &&
                buttons[gridSize + 1].Text == symbol &&
                buttons[2 * gridSize].Text == symbol)
            {
                return true;
            }

            return false;
        }


        private void ResetGame()
        {
            foreach (Button button in buttons)
            {
                button.Enabled = true;
                button.Text = "";
            }

            isPlayerX = true;
            IsComputerTurn = false;
        }
    }
}
