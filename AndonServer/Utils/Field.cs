using AndonServer;
using AndonServer.Models;
using AndonServer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AndonModbus.Utils
{
    class Field
    {
        public static DispatcherTimer time = new DispatcherTimer();
        public static Dictionary<int, string> btnType = new Dictionary<int, string> {
            {1, "NotGood"}, {2, "Repair"}, {3, "Downtime"}, {4, "NGHistory" }, { 5, "MACHINE"}, {6, "SHIFT"}, {7, "RESET"}
        };
        public static List<Machine> machine = new List<Machine>();
        public static List<Machine> mdown = new List<Machine>();
        public static Random r = new Random();
        public static System.Windows.Controls.TextBlock status = null;
        public static Brush black;
        public static Brush red;
        public static Brush red2;
        public static Brush white;
        public static WEB web = null;
        public static BackgroundWorker bw = new BackgroundWorker();

        public static void IClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int index = -1;
            int.TryParse(((Grid)sender).Tag.ToString(), out index);
            try
            {
                new DetailUI(machine[index]).ShowDialog();
            }
            catch (Exception ea)
            {
                MessageBox.Show(ea.ToString());
            }
        }
        //public static MQTT mqtt = null;
    }
}
