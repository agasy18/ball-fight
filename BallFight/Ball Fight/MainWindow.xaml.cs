using BallNetModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
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
            //BallPosition = new Vector(Ball.Margin.Left, Ball.Margin.Top);
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
            //Ball.Margin = BallMagine;
        }

        private void SendMessage(Thickness mar)
        {
            if (remoteProxy == null|| clientUser ==null) return;
            Task.Run(() =>
            {
                try
                {
                    //////////////////////////////////////////////////////////////////////////
                    BallMessage newMessage = new BallMessage()
                    {
                        Date = DateTime.Now,
                        Message = "", //player's coordinates
                        User = clientUser,
                        Margine = mar
                    };
                    remoteProxy.SendNewMessage(newMessage);
                    NetworkError = false;
                }
                catch(Exception ex)
                {
                    ex.DebugDesc().Log("SendMargine");
                    NetworkError = true;
                }
            });
        }

        public string AppAddress
        {
            get
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();

                string MACAddress = String.Empty;

                foreach (ManagementObject mo in moc)
                {
                    if (MACAddress == String.Empty)
                    { // only return MAC Address from first card
                        if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                    }
                    mo.Dispose();
                }

                return MACAddress + Process.GetCurrentProcess().Id;
            }
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
                else
                    return;
               
            }
        }
        
        private void GameBoard_LayoutUpdated(object sender, EventArgs e)
        {
            //Ball.Margin=BallMagine;
        }
        
        private void CurWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EndButtonPressed(sender, null);     
        }

        
        
        private async void CurWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (First)
            {
                First = false;
                //ResetBallParams();
                LoadUpdateSystem();

                // Start();        
                
               
            }
        }

        
        private void LoadUpdateSystem()
        {
            Timer netTimer;
            netTimer = new Timer(1000/60);
            netTimer.Elapsed += (s, q) =>
                 {
                     if (clientUser == null)
                     {
                         return;
                     }
                     Dispatcher.BeginInvoke(new Action(() =>
                     {
                         SendMessage(Player1.Margin);
                     }));
                     try
                     {
                         //////////////////////////////////////////////////////////////////////////
                         List<BallMessage> messages = remoteProxy.GetNewMessages(clientUser);
                         if (messages != null)
                         {

                             foreach (var message in messages)
                                 Dispatcher.BeginInvoke(new Action(() =>
                                   {
                                       if (!IsGameStarted)
                                       {
                                           StartGame(message);
                                           IsGameStarted = true;
                                       }
                                       var mar= Player2.Margin;
                                       mar.Left = message.Margine.Left;
                                       Player2.Margin = mar;
                                       Ball.Margin = message.BallMargine;
                                       GameSpeedLabel.Content = message.GameSpeed;
                                   }));

                         }
                         NetworkError = false;

                     }
                     catch (Exception e)
                     {
                         e.DebugDesc().Log("ChackNet");
                         NetworkError = true;
                     }

                 };
            netTimer.Start();
        }

        private void StartGame(BallMessage message)
        {
            //BallPosition = new Vector(GameBoard.Width / 2, GameBoard.Height / 2);
            //gameSpeed = 1;
            Player2Label.Content = message.User.UserName;
            //ballAngel = (string.Compare(message.User.AppAddress, clientUser.AppAddress) > 0 ? -1 : 1) * Math.PI / 3;
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
            string userName = PlayerTextBox.Text;
            string keyText = KeyTextBox.Text;
            Task.Run(() =>
                {
                    try
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblStatus.Content = "Connecting...";
                        }));
                        if (!String.IsNullOrEmpty(userName))
                        {
                            remoteFactory = new ChannelFactory<IBallService>("BallConfig");
                            remoteProxy = remoteFactory.CreateChannel(); 
                            clientUser = remoteProxy.ClientConnect(userName, AppAddress, keyText);

                            if (clientUser != null)
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ConfigConnectedState();
                                    lblStatus.Content = "Connected as: " + clientUser.UserName;
                                }));
                            }
                        }
                        else
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                lblStatus.Content = "Disconnected";
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugDesc().Log("Start");           
                    }
                });
        }

        private void ConfigConnectedState()
        {
            
        }

        public IBallService remoteProxy { get; set; }

        public BallUser clientUser { get; set; }

        public bool NetworkError { get; set; }

        private void EndButtonPressed(object sender, RoutedEventArgs e)
        {
            if (!NetworkError)
            {
                Task.Run(() =>
                {
                    try
                    {
                        remoteProxy.RemoveUser(clientUser);
                    }
                    catch(Exception ex)
                    {
                        ex.DebugDesc().Log("EndButtonPressed");       
                    }
                });
            }
        }

        public bool IsGameStarted { get; set; }
    }
}
