using BallNetModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ball_Fight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        const double KeyDelta = 10;
        private bool First = true;
        public MainWindow()
        {
            InitializeComponent();
            BallPosition = new Vector(Ball.Margin.Left, Ball.Margin.Top);
            PlayerTextBox.Text = Environment.UserName;
        }

        private void Canvas_MouseEnter_1(object sender, MouseEventArgs e)
        {
            
        }

        private void Canvas_MouseLeave_1(object sender, MouseEventArgs e)
        {

        }
        
        private void Canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
            var mar = Player1.Margin;
            mar.Left = Math.Min(Math.Max(0, e.GetPosition(GameBoard).X - Player1.Width / 2), GameBoard.Width - Player1.Width);
            Player1.Margin = mar;
            Ball.Margin = BallMagine;
        }

        private void CurWindow_Activated(object sender, EventArgs e)
        {
            Continue();
        }

        private void Continue()
        {
        }

        private void CurWindow_Deactivated(object sender, EventArgs e)
        {
            Pause();
        }

        private void Pause()
        {
        }

        private void CurWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Key == Key.Left)
                {
                    var mar = Player1.Margin;
                    mar.Left = Math.Min(Math.Max(0, mar.Left - KeyDelta), GameBoard.Width - Player1.Width);
                    Player1.Margin = mar;
                }
                else if (e.Key == Key.Right)
                {
                    var mar = Player1.Margin;
                    mar.Left = Math.Min(Math.Max(0, mar.Left + KeyDelta), GameBoard.Width - Player1.Width);
                    Player1.Margin = mar;
                }
            }
        }
        int fpsCounter = 0;
        private void GameBoard_LayoutUpdated(object sender, EventArgs e)
        {
            fpsCounter++;
            Ball.Margin=BallMagine;           
        }
        
        private void CurWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        bool diasbleTimer = false;
        
        private async void CurWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (First)
            {
                First = true;
                LoadUpdateSystem();
                LoadSpeedSystem();
                LoadFpsSystem();
                Start();        
                for (; ;)
                {
                    if (diasbleTimer == false)
                    {
                        for (int i = 0; i < gameSpeed; i++)                        
                        tick();
                    }
                    await Task.Delay(1);
                }
               
            }
        }

        private void LoadFpsSystem()
        {
            Timer fpsTimer;
            fpsTimer = new Timer(1000);
            fpsTimer.Elapsed += (s, q) =>
            {
                int fps = fpsCounter;
                fpsCounter = 0;
                fpsLabel.Dispatcher.BeginInvoke(new Action(() =>
                {
                    fpsLabel.Content ="FPS: " + fps;
                }));
            };
            fpsTimer.Start();
        }
        private void LoadUpdateSystem()
        {
            Timer fpsTimer;
            fpsTimer = new Timer(1000/60);
            fpsTimer.Elapsed += (s, q) =>
            {
                fpsLabel.Dispatcher.BeginInvoke(new Action(() =>
                {
                    GameBoard_LayoutUpdated(null,null);
                }));
            };
            
            fpsTimer.Start();
        }
        private void LoadSpeedSystem()
        {
            Timer fpsTimer;
            fpsTimer = new Timer(1000 * 2);
            GameSpeedLabel.Content = gameSpeed;
            fpsTimer.Elapsed += (s, q) =>
            {
                fpsLabel.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (gameSpeed > 10) 
                        return;
                    gameSpeed++;
                    
                    GameSpeedLabel.Content = gameSpeed;
                }));
            };

            fpsTimer.Start();
        }

        int gameSpeed = 1;
        double ballAngel;
        const double ballV = 1;

        Vector BallPosition;
        Thickness BallMagine
        {
            get
            {
                return new Thickness(BallPosition.X, BallPosition.Y, BallPosition.X + Ball.Width, BallPosition.Y + Ball.Height);
            }
        }
        Vector BallVelocity
        {
            get
            {
                var dx = ballV * Math.Sin(ballAngel);
                var dy = ballV * Math.Cos(ballAngel); 
                return new Vector(dx, dy);
            }
        }
        private void tick()
        {
            BallPosition += BallVelocity;
            var mar = BallMagine;         
            if (mar.Left <= 0)
            {
                ballAngel = -ballAngel;
            }
            else if(mar.Right >= GameBoard.Width )
            {
                ballAngel = -ballAngel;
            }

            else if (mar.Top <= Player2.Margin.Top + Player2.Height && mar.Right - 2 > Player2.Margin.Left && mar.Left + 2 < Player2.Margin.Left + Player2.Width)
            {
                ballAngel = Math.PI - ballAngel;
            }
            else if (mar.Bottom >= Player1.Margin.Top && mar.Right-2>Player1.Margin.Left && mar.Left+2<Player1.Margin.Left+Player1.Width)
            {
                ballAngel = Math.PI - ballAngel;
            }
            else if (mar.Top <= 0)
            {
                GameOver(Player2Label.Content as string);
                ballAngel = Math.PI - ballAngel;
            }
            else if (mar.Bottom >= GameBoard.Height )
            {
                GameOver(PlayerTextBox.Text);
                ballAngel = Math.PI - ballAngel;
            }
        }

        private void GameOver(string p)
        {
            p.Log("Game Over");
        }

     
        


        private void PlayerTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void KeyTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }
        private ChannelFactory<IBallService> remoteFactory;
        private void Start()
        {
            ballAngel =- Math.PI / 3;
            try
            {
                lblStatus.Content = "Connecting...";
                if (!String.IsNullOrEmpty(PlayerTextBox.Text))
                {
                    remoteFactory = new ChannelFactory<IBallService>("BallConfig");
                    remoteProxy = remoteFactory.CreateChannel();
                    clientUser = remoteProxy.ClientConnect(PlayerTextBox.Text,KeyTextBox.Text);

                    if (clientUser != null)
                    {
                        ConfigConnectedState();
                        lblStatus.Content = "Connected as: " + clientUser.UserName;
                    }
                }
                else
                    lblStatus.Content = "Disconnected";
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has ocurred\nClient cannot connect\nMessage:" + ex.Message,
                    "FATAL ERROR", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void ConfigConnectedState()
        {

        }

        public IBallService remoteProxy { get; set; }

        public BallUser clientUser { get; set; }
    }
}
