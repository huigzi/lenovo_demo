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

        public MainWindow()
        {
            InitializeComponent();

            var builder = new ContainerBuilder();

            builder.RegisterType<UsbServer>().As<IService>().SingleInstance();
            builder.RegisterType<ReadConfiguration>().As<IReadFile>().SingleInstance();
            builder.RegisterType<GestureAndPresenceMethod>().As<IAlgorithmMethod<float>>().SingleInstance();
            builder.RegisterType<AlgorithmFlow>().As<IAlgorithmFlow<short[]>>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<SaveData>().As<ISaveData<byte[]>>().SingleInstance();
            builder.RegisterType<StatusMachine>().SingleInstance();
            builder.Register(c => new ActionBlock<State>(c.Resolve<StatusMachine>().StateChanged)).SingleInstance();
            builder.RegisterType<DataFlowBlock>().SingleInstance();
            builder.RegisterType<MainViewModel>().SingleInstance();

            container = builder.Build();

            DataContext = container.Resolve<MainViewModel>();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            try
            {
                container.Resolve<IService>().OpenServer();
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

    }
}

