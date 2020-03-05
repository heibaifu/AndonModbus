using AndonModbus.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AndonServer
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
            Field.web = new Utils.WEB();
            Field.time.Interval = TimeSpan.FromSeconds(1);
            Field.time.Start();
        }
    }
}
