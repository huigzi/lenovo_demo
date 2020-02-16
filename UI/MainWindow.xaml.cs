using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Algorithm;
using Autofac;
using Core;
using Core.Interface;

namespace UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private readonly IContainer container;
        private readonly MusicPlayer musicPlayer;

        public MainWindow()
        {
            InitializeComponent();

            var builder = new ContainerBuilder();
            builder.RegisterType<UsbServer>().As<IService>().SingleInstance();
            builder.RegisterType<ReadConfiguration>().As<IReadFile>().SingleInstance();
            builder.RegisterType<GestureAndPresenceMethod>().As<IAlgorithmMethod<float>>().SingleInstance();
            builder.RegisterType<AlgorithmFlow>().As<IAlgorithmFlow<short[]>>().SingleInstance();
            builder.RegisterType<MusicPlayer>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<SaveData>().As<ISaveData<byte[]>>().SingleInstance();

            var statusMachine = new StatusMachine();

            var uiActionBlock = new ActionBlock<ArrayList>(x =>
            {
                //lampFlag = 1 - lampFlag;

                //Dispatcher?.InvokeAsync(() =>
                //{
                //    //RecvLamp.Background = lampFlag == 1
                //    //    ? new SolidColorBrush(Color.FromRgb(255, 255, 0))
                //    //    : new SolidColorBrush(Color.FromRgb(0, 255, 255));

                //    //if((State) x[0] == State.SomeOne)
                //    //{
                //    //    Rline.PlotY(x[1] as IEnumerable);
                //    //    Sline.PlotY(x[2] as IEnumerable);
                //    //}

                //});

                if ((State) x[0] == statusMachine.LastState) return;
                statusMachine.LastState = (State) x[0];
                StateChangeUi(x);
            });

            builder.RegisterInstance(uiActionBlock).As<ActionBlock<ArrayList>>().SingleInstance();
            builder.RegisterType<DataFlowBlock>().SingleInstance();

            container = builder.Build();

            musicPlayer = container.BeginLifetimeScope().Resolve<MusicPlayer>();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            Screen.Visibility = Visibility.Hidden;
            Screen.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            WindowState = WindowState.Maximized;

            LastM.Content = musicPlayer.MusicList[0];
            CurrentM.Content = musicPlayer.MusicList[1];
            NextM.Content = musicPlayer.MusicList[2];

            try
            {
                container.BeginLifetimeScope().Resolve<IService>().OpenServer();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            container.Resolve<ILogger>().ShutDown();
            container.Resolve<IService>().CloseServer();
            container.Dispose();
            Environment.Exit(0);
        }

        private void StateChangeUi(IList arrayList)
        {
            switch ((State)arrayList[0])
            {
                case State.NoOne:
                    Dispatcher?.InvokeAsync(() => { Screen.Visibility = Visibility.Visible; });
                    break;

                case State.SomeOne:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        Screen.Visibility = Visibility.Hidden;
                    });
                    break;

                case State.DoubleClickMiddle:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        //Rline.PlotY(arrayList[1] as IEnumerable);
                        //Sline.PlotY(arrayList[2] as IEnumerable);

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
                        //Rline.PlotY(arrayList[1] as IEnumerable);
                        //Sline.PlotY(arrayList[2] as IEnumerable);

                        musicPlayer.UpdateVolume();
                        Pb.Value = musicPlayer.Volume;
                    });
                    break;

                case State.LeftSweep:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        musicPlayer.ListRight();

                        LastM.Content = musicPlayer.MusicList[0];
                        CurrentM.Content = musicPlayer.MusicList[1];
                        NextM.Content = musicPlayer.MusicList[2];
                        //Rline.PlotY(arrayList[1] as IEnumerable);
                        //Sline.PlotY(arrayList[2] as IEnumerable);

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
                        //Rline.PlotY(arrayList[1] as IEnumerable);
                        //Sline.PlotY(arrayList[2] as IEnumerable);

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
                    
                    //Dispatcher?.InvokeAsync(() =>
                    //{
                    //    Rline.PlotY(arrayList[1] as IEnumerable);
                    //    Sline.PlotY(arrayList[2] as IEnumerable);
                    //});
                    break;

                default:
                    break;
            }
        }

        private void ScreenDoubleClick(object sender, EventArgs e)
        {
            GestureR.Visibility = GestureR.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            GestureS.Visibility = GestureS.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            SaveDataButton.Visibility =
                SaveDataButton.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void SaveDataButton_Click(object sender, RoutedEventArgs e)
        {
            var saveData = container.Resolve<ISaveData<byte[]>>();

            if (saveData.State == DataState.Started)
            {
                saveData.StopSave();
                SaveDataButton.Content = "开始保存";
            }
            else
            {
                saveData.StartSave();
                SaveDataButton.Content = "停止保存";
            }
        }
    }
}

