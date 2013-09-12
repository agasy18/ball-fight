using System;
using System.Collections.Generic;
using System.Linq;
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
        Storyboard BallAnimation = new Storyboard();
        private bool First = true;
        public MainWindow()
        {
            InitializeComponent();
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

        private void GameBoard_LayoutUpdated(object sender, EventArgs e)
        {

        }

        private void CurWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {


        }
        Timer timer = new Timer(10);
        private void CurWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (First)
            {
                First = true;

                timer.Elapsed += (s, v) =>
                {
                    try
                    {
                        this.Dispatcher.Invoke(new Action(() => { tick(); }));
                    }
                    catch
                    { }
                };
                timer.Enabled = false;
                Start();
            }
        }
        double ballAngel;
        const double ballV = 2;
        private void tick()
        {
            var mar = Ball.Margin;
            mar.Left += ballV * Math.Sin(ballAngel);
            mar.Top += ballV * Math.Cos(ballAngel);
            if (mar.Left <= 0 || mar.Left>= GameBoard.Width-Ball.Width )
            {
                ballAngel = - ballAngel;
                tick();
                return;
            }
                        if (Kpav(mar,Player1) || Kpav (mar,Player2))
            {
                ballAngel = Math.PI- ballAngel;
                tick();
                return;
            }
           
            Ball.Margin = mar;
        }

        private bool Kpav(Thickness mar, Rectangle Player)
        {
            return mar.Top <= 0 || mar.Top >= GameBoard.Height - Ball.Height;
           
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

        private void Start()
        {
            timer.Enabled = true;
            ballAngel =- Math.PI / 3;
        }
    }
}
