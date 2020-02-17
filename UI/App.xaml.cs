using System.Windows;

namespace UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var window = new MainWindow
            {
                WindowState = WindowState.Maximized
            };

            window.Show();
        }
    }
}
