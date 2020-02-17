using Algorithm;
using Autofac;
using Core;
using Core.Interface;
using Core.ViewModel;
using System;
using System.Collections;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;

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
        private readonly StatusMachine statusMachine;

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
            builder.Register(c => new ActionBlock<ArrayList>(StateChangeUi)).SingleInstance();
            builder.RegisterType<DataFlowBlock>().SingleInstance();
            builder.RegisterType<MainViewModel>().SingleInstance();

            container = builder.Build();

            statusMachine = new StatusMachine();

            musicPlayer = container.BeginLifetimeScope().Resolve<MusicPlayer>();
            DataContext = container.Resolve<MainViewModel>();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
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

        public void StateChangeUi(IList arrayList)
        {
            //Dispatcher?.InvokeAsync(() =>
            //{
            //    Rline.PlotY(arrayList[1] as IEnumerable);
            //    Sline.PlotY(arrayList[2] as IEnumerable);
            //});

            if ((State)arrayList[0] == statusMachine.LastState) return;

            switch ((State)arrayList[0])
            {
                case State.NoOne:
                    Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    break;

                case State.SomeOne:
                    Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    break;

                case State.DoubleClickMiddle:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        switch (musicPlayer.CurrentStatus)
                        {
                            case PlayerState.Start:
                                musicPlayer.Pause();
                                musicPlayer.CurrentStatus = PlayerState.Pause;
                                break;
                            case PlayerState.Pause:
                                musicPlayer.Resume();
                                musicPlayer.CurrentStatus = PlayerState.Start;
                                break;
                            case PlayerState.Stop:
                                musicPlayer.Index = 1;
                                musicPlayer.Play();
                                musicPlayer.CurrentStatus = PlayerState.Start;
                                break;
                            default:
                                break;
                        }

                    });
                    break;

                case State.Circle:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        musicPlayer.UpdateVolume();
                        Pb.Value = musicPlayer.Volume;
                    });
                    break;

                case State.LeftSweep:

                    Dispatcher?.InvokeAsync(() =>
                    {
                        musicPlayer.ListRight();

                        switch (musicPlayer.CurrentStatus)
                        {
                            case PlayerState.Start:
                                musicPlayer.Stop();
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

                        switch (musicPlayer.CurrentStatus)
                        {
                            case PlayerState.Start:
                                musicPlayer.Stop();
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
                    
                    break;

                default:
                    break;
            }
        }
    }
}

