using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class MainForm : Form
    {
        private bool isPlayerX;
        private bool isComputerTurn;

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
                button.Size = new Size(buttonSize, buttonSize);
                button.Font = new Font("Arial", 36);
                button.Tag = i;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = Color.Black;
                button.Click += Button_Click;

                int row = i / gridSize;
                int col = i % gridSize;
                button.Location = new Point(padding + col * (buttonSize + padding), padding + row * (buttonSize + padding));

                buttons.Add(button);

                Controls.Add(button);
            }

            isPlayerX = true;
            isComputerTurn = false;
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

        private void MakeComputerMove()
        {
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
                int bestScore = int.MinValue;
                Button bestMove = availableButtons[0];

                foreach (Button button in availableButtons)
                {
                    button.Text = "O";
                    int score = AlphaBeta(0, int.MinValue, int.MaxValue, false);
                    button.Text = "";

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = button;
                    }
                }

                bestMove.Text = "O";
                bestMove.Enabled = false;
            }

            if (IsBoardFull() && !CheckForWin("X") && !CheckForWin("O"))
            {
                MessageBox.Show("Ничья!");
                ResetGame();
            }

            isPlayerX = true;
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
            isComputerTurn = false;
        }
    }
}
