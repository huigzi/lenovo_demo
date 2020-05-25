using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Algorithm;
using Autofac;
using Core;
using Core.Interface;
using Core.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace CollectionUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IContainer container;

        public MainWindow(string name, string path )
        {
            InitializeComponent();

            var builder = new ContainerBuilder();

            builder.RegisterType<UsbServer>().As<IService>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.Register(c => new SaveData(name, path)).As<ISaveData<byte[]>>().SingleInstance();
            builder.Register(c => new ActionBlock<byte[]>(c.Resolve<ISaveData<byte[]>>().WriteProcess)).SingleInstance();
            builder.RegisterType<CollectionDataFlow>().As<IDataFlow>().SingleInstance();
            builder.RegisterType<MainViewModel>().SingleInstance();

            container = builder.Build();

            DataContext = container.Resolve<MainViewModel>();

            Messenger.Default.Register<int>(this, "LeftMove", LeftMoving);
            Messenger.Default.Register<int>(this, "RightMove", RightMoving);
            Messenger.Default.Register<int>(this, "DoubleLeftMove", DoubleLeftMoving);
            Messenger.Default.Register<int>(this, "DoubleRightMove", DoubleRightMoving);
            Messenger.Default.Register<byte[]>(this, "ChangeBackground", ChangeBackground);
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            try
            {
                container.Resolve<IService>().OpenServer();
            }
            catch (Exception ex)
            {
                Messenger.Default.Send("请检查设备连接", "PresentUpdate");
            }
        }

        private void LeftMoving(int a)
        {
            Dispatcher.Invoke(() => { LeftMove.Begin(); });
        }

        private void RightMoving(int a)
        {
            Dispatcher.Invoke(() => { RightMove.Begin(); });
        }

        private void DoubleLeftMoving(int a)
        {
            Dispatcher.Invoke(() => { LeftDouble.Begin(); });
        }

        private void DoubleRightMoving(int a)
        {
            Dispatcher.Invoke(() => { RightDouble.Begin(); });
        }

        private void ChangeBackground(byte[] data)
        {
            Dispatcher.Invoke(
                () => { Grid.Background = new SolidColorBrush(Color.FromRgb(data[0], data[1], data[2])); });

        }

    }
}
