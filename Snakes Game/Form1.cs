using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Imaging; // for the JPG compressor.

namespace Snakes_Game
{
    public partial class Form1 : Form
    {

        private List<circle> snake = new List<circle>();

        private circle food = new circle();

        int maxWidth;
        int maxHeight;

        int score;
        int highscore;

        Random rand = new Random();

        bool goLeft, goRight, goUp, goDown;


        public Form1()
        {
            InitializeComponent();

            new Settings();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Left && Settings.directions != "right")
            {
                goLeft = true;
            }
            if(e.KeyCode == Keys.Right && Settings.directions != "left")
            {
                goRight = true;
            }
            if(e.KeyCode == Keys.Up && Settings.directions != "down")
            {
                goUp = true;
            }
            if(e.KeyCode == Keys.Down && Settings.directions != "up")
            {
                goDown = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void TakeSnapShot(object sender, EventArgs e)
        {
            Label caption = new Label();
            caption.Text = "I scored: " + score + " and my Highscore is " + highscore + " in Snakegames.";
            caption.Font = new Font("Ariel", 12, FontStyle.Bold); 
            caption.ForeColor = Color.Black;
            caption.AutoSize = false;
            caption.Width = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Snakegame Snapshot";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;    

            if(dialog.ShowDialog() == DialogResult.OK)
            { 
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);
            }
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            // setting the directions

            if (goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goDown)
            {
                Settings.directions = "down";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }
            // end of direction


            for(int i = snake.Count - 1; i >= 0; i--)
            {
                if(i == 0)
                {
                    switch ( Settings.directions )
                    {
                        case "left":
                            snake[i].x--;
                            break;
                        case "right":
                            snake[i].x++;
                            break;
                        case "down":
                            snake[i].y++;
                            break;
                        case "up":
                            snake[i].y--;
                            break;
                    }

                    if(snake[i].x < 0)
                    {
                        snake[i].x = maxWidth;
                    }
                    if(snake[i].x > maxWidth)
                    {
                        snake[i].x = 0;
                    }
                    if(snake[i].y < 0)
                    {
                        snake[i].y = maxHeight;
                    }
                    if(snake[i].y > maxHeight)
                    {
                        snake[i].y = 0;
                    }


                     if(snake[i].x == food.x && snake[i].y == food.y)
                     {
                        EatFood(); 
                     }

                     for(int j = 1; j < snake.Count; j++)
                     {
                        if(snake[i].x == snake[j].x && snake[i].y == snake[j].y)
                        {
                            GameOver();
                        }
                     }  
                }
                else
                {
                    snake[i].x = snake[i - 1].x;
                    snake[i].y = snake[i - 1].y;
                }
            }


            picCanvas.Invalidate();
        }

        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            Brush snakeColour;

            for(int i=0; i<snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                {
                    snakeColour = Brushes.DarkGreen;
                }

                canvas.FillEllipse(snakeColour, new Rectangle
                    (
                    snake[i].x * Settings.width,
                    snake[i].y * Settings.height,
                    Settings.width  , Settings.height
                    )); 
            }
            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
                (
                food.x * Settings.width,
                food.y * Settings.height,
                Settings.width, Settings.height
                ));
        }

        private void txtScore_Click(object sender, EventArgs e)
        {

        }

        private void RestartGame()
        {
            maxWidth = picCanvas.Width / Settings.width - 1;
            maxHeight = picCanvas.Height / Settings.height - 1;

            snake.Clear();

            StartButton.Enabled = false;
            SnapButton.Enabled = false;

            score = 0;
            txtScore.Text = "Score: " + score;

            circle head = new circle { x = 10, y = 5 };
            snake.Add(head); // adding head of snake to the list.

            for (int i = 0; i < 10; i++)
            {
                circle body = new circle();
                snake.Add(body);
            }

            food = new circle { x = rand.Next(2, maxWidth), y = rand.Next(2, maxHeight) };

            gameTimer.Start();
        }

        private void EatFood()
        {
            score += 1;
            txtScore.Text = "Score: " + 1;

            circle body = new circle
            {
                x = snake[snake.Count - 1].x,
                y = snake[snake.Count - 1].y
            };

            snake.Add(body);

            food = new circle { x = rand.Next(2, maxWidth), y = rand.Next(2, maxHeight) };
        }

        private void GameOver()
        {
            gameTimer.Stop();
            StartButton.Enabled = true;
            SnapButton.Enabled = true;

            if(score > highscore)
            {
                highscore = score;
                txtHighScore.Text = "High Score: " + Environment.NewLine + highscore;
                txtHighScore.ForeColor = Color.Maroon;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;
            }
        } 
    }
}
