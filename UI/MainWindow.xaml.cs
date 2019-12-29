using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Algorithm;
using Core;

namespace UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private readonly MusicPlayer musicPlayer;
        private readonly UsbServer usbServer;
        private readonly Logger logger;
        private int lampFlag = 0;

        public MainWindow()
        {
            InitializeComponent();

            logger = new Logger();

            musicPlayer = new MusicPlayer();
            musicPlayer.PlayerInitial("134");


            var readXml = new ReadXml();
            var process = new Process(new GestureAndPresenceMethod(readXml), readXml);
            var statusMachine = new StatusMachine();

            var transformBlock = new TransformBlock<byte[], ArrayList>(x => process.DataProcess(x));
            var actionBlock = new ActionBlock<ArrayList>(x =>
            {
                lampFlag = 1 - lampFlag;

                Dispatcher?.InvokeAsync(() =>
                {
                    RecvLamp.Background = lampFlag == 1
                        ? new SolidColorBrush(Color.FromRgb(255, 255, 0))
                        : new SolidColorBrush(Color.FromRgb(0, 255, 255));
                });

                if ((State) x[0] == statusMachine.LastState) return;
                statusMachine.LastState = (State) x[0];
                StateChangeUi(x);
            });

            transformBlock.LinkTo(actionBlock);

            usbServer = new UsbServer(transformBlock);

        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            Screen.Visibility = Visibility.Hidden;
            Screen.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            WindowState = WindowState.Maximized;
            LastM.Content = musicPlayer.MusicList[0];
            CurrentM.Content = musicPlayer.MusicList[1];
            NextM.Content = musicPlayer.MusicList[2];

            Task.Run(() =>
                {
                    var res = usbServer.UsbFind();
                    {
                        if (res == -1)
                        {
                            MessageBox.Show("USB Device Not Find.");
                            logger.WriteToLog("USB Device Not Find.");
                            return;
                        }
                    }
                    usbServer.Initialize();
                }
            );
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            logger.ShutDown();
            usbServer.CloseUsb();
            Environment.Exit(0);
        }

        private void StateChangeUi(ArrayList arrayList)
        {
            switch ((State)arrayList[0])
            {
                case State.NoOne:
                    Dispatcher?.InvokeAsync(() => { Screen.Visibility = Visibility.Visible; });
                    break;

                case State.SomeOne:
                    Dispatcher?.InvokeAsync(() => { Screen.Visibility = Visibility.Hidden; });
                    break;

                case State.DoubleClick:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        Rline.PlotY((IEnumerable)arrayList[1]);
                        Sline.PlotY((IEnumerable)arrayList[2]);

                        switch (musicPlayer.CurrentStatus)
                        {
                            case PlayerState.Start:
                                musicPlayer.Pause();
                                musicPlayer.CurrentStatus = PlayerState.Pause;
                                CurrentI.Source =
                                    new BitmapImage(new Uri(Path.GetFullPath("./Resource/stop.png")));
                                break;
                            case PlayerState.Pause:
                                musicPlayer.Resume();
                                musicPlayer.CurrentStatus = PlayerState.Start;
                                CurrentI.Source =
                                    new BitmapImage(new Uri(Path.GetFullPath("./Resource/start.png")));
                                break;
                            case PlayerState.Stop:
                                musicPlayer.Index = 1;
                                musicPlayer.Play();
                                musicPlayer.CurrentStatus = PlayerState.Start;
                                CurrentI.Source =
                                    new BitmapImage(new Uri(Path.GetFullPath("./Resource/start.png")));
                                break;
                            default:
                                break;
                        }

                    });
                    break;

                case State.Circle:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        Rline.PlotY((IEnumerable)arrayList[1]);
                        Sline.PlotY((IEnumerable)arrayList[2]);

                        musicPlayer.UpdateVolume();
                        Pb.Value = musicPlayer.Volume;
                    });
                    break;

                case State.LeftSweep:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        musicPlayer.ListLeft();

                        LastM.Content = musicPlayer.MusicList[0];
                        CurrentM.Content = musicPlayer.MusicList[1];
                        NextM.Content = musicPlayer.MusicList[2];
                        Rline.PlotY((IEnumerable)arrayList[1]);
                        Sline.PlotY((IEnumerable)arrayList[2]);

                        switch (musicPlayer.CurrentStatus)
                        {
                            case PlayerState.Start:
                                musicPlayer.Stop();
                                musicPlayer.Index = 1;
                                musicPlayer.Play();
                                break;
                            case PlayerState.Pause:
                                musicPlayer.Stop();
                                musicPlayer.CurrentStatus = PlayerState.Stop;
                                break;
                        }
                    });

                    break;

                case State.RightSweep:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        musicPlayer.ListRight();

                        LastM.Content = musicPlayer.MusicList[0];
                        CurrentM.Content = musicPlayer.MusicList[1];
                        NextM.Content = musicPlayer.MusicList[2];
                        Rline.PlotY((IEnumerable)arrayList[1]);
                        Sline.PlotY((IEnumerable)arrayList[2]);

                        switch (musicPlayer.CurrentStatus)
                        {
                            case PlayerState.Start:
                                musicPlayer.Stop();
                                musicPlayer.Index = 1;
                                musicPlayer.Play();
                                break;
                            case PlayerState.Pause:
                                musicPlayer.Stop();
                                musicPlayer.CurrentStatus = PlayerState.Stop;
                                break;
                        }
                    });
                    break;

                case State.OtherGesture:
                    
                    Dispatcher?.InvokeAsync(() =>
                    {
                        Rline.PlotY((IEnumerable)arrayList[1]);
                        Sline.PlotY((IEnumerable)arrayList[2]);
                    });
                    break;

                default:
                    break;
            }
        }

        private void ScreenDoubleClick(object sender, EventArgs e)
        {
            GestureR.Visibility = GestureR.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            GestureS.Visibility = GestureS.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
    }
}

