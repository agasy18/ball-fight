using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace BallNetModel
{
    class BallModel
    {
        public Size BallSize { get; private set; }
        public Vector BallPosition { get; private set; }
        public Size BoardSize { get; private set; }

        int gameSpeed = 1;

        public int GameSpeed
        {
            get { return gameSpeed; }
            set { gameSpeed = value; }
        }
        double ballAngel;
        const double ballV = 1;

        public Thickness Player1Margine { get; set; }
        public Thickness Player2Margine { get; set; }
        public Size PlayerSize { get; private set; }
        bool diasbleTimer = false;

        Vector BallVelocity
        {
            get
            {
                var dx = ballV * gameSpeed * Math.Sin(ballAngel);
                var dy = ballV * gameSpeed * Math.Cos(ballAngel);
                return new Vector(dx, dy);
            }
        }

        public Thickness BallMagine
        {
            get
            {
                return new Thickness(BallPosition.X, BallPosition.Y, BallPosition.X + BallSize.Width, BallPosition.Y + BallSize.Height);
            }
        }

        public BallModel()
        {
            ResetBoardParams();
            ResetBallParams();
            LoadSpeedSystem();

            StartTimerManager();
        }

        private void tick()
        {
            BallPosition += BallVelocity;
            var mar = BallMagine;
            if (mar.Left <= 0)
            {
                ballAngel = -ballAngel;
            }
            else if (mar.Right >= BoardSize.Width)
            {
                ballAngel = -ballAngel;
            }

            else if (mar.Top <= Player2Margine.Top + PlayerSize.Height && mar.Right - 2 > Player2Margine.Left && mar.Left + 2 < Player2Margine.Left + PlayerSize.Width)
            {
                ballAngel = Math.PI - ballAngel;
            }
            else if (mar.Bottom >= Player1Margine.Top && mar.Right - 2 > Player1Margine.Left && mar.Left + 2 < Player1Margine.Left + PlayerSize.Width)
            {
                ballAngel = Math.PI - ballAngel;
            }
            else if (mar.Top <= 0)
            {
                GameOver(false);
                ballAngel = Math.PI - ballAngel;
            }
            else if (mar.Bottom >= BoardSize.Height)
            {
                GameOver(true);
                ballAngel = Math.PI - ballAngel;
            }
        }

        private void GameOver(bool I)
        {
            ResetBallParams();
        }

        private void ResetBallParams()
        {
            ballAngel = Math.PI / 3;
            gameSpeed = 1;
            
            BallPosition = new Vector(BoardSize.Width / 2, BoardSize.Height / 2);
        }

        private void ResetBoardParams()
        {
            BallSize = new Size(18, 18);
            BoardSize = new Size(301, 429);
            PlayerSize = new Size(72, 10);

            Player1Margine = new Thickness(10, 409, 0, 0);
            Player2Margine = new Thickness(BoardSize.Width - (PlayerSize.Width / 2 + 10), 10, 0, 0);
        }

        private void LoadSpeedSystem()
        {
            Timer fpsTimer;
            fpsTimer = new Timer(1000 * 20);
            
            fpsTimer.Elapsed += (s, q) =>
            {
                if (gameSpeed > 10)
                    return;
                gameSpeed++;
            };

            fpsTimer.Start();
        }

        private async void StartTimerManager()
        {
            for (; ; )
            {
                if (diasbleTimer == false)
                {
                    
                    tick();
                }
                await Task.Delay(10);
            }
        }
    }
}
