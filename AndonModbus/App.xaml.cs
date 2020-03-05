using AndonModbus.Utils;
using System.Windows;
using System.Windows.Media;

namespace AndonModbus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Field.white = new SolidColorBrush(Colors.White);
            Field.black = new SolidColorBrush(Colors.Black);
            Field.red = new SolidColorBrush(Colors.Red);
            Field.red2 = (Brush)new BrushConverter().ConvertFromString("#DE0612");
            Field.web = new WEB(null);
            Field.bw.DoWork += Field.sendData;
            Field.mqtt = new MQTT();
            Field.help = new Helper();
            Field.server = new WEBS();
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }
    }
}
