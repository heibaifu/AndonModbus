using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AndonModbus.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        string regport = "[0-9]+$";
        Properties.Settings s = Properties.Settings.Default;
        public Settings()
        {
            InitializeComponent();
            host.Text = s.host;
            prefix.Text = s.prefix;
            hport.Text = s.httpPort.ToString();
            mport.Text = s.MqttPort.ToString();
            sport.Text = s.ServerPort.ToString();
            //var prop = Properties.Settings.Default.GetType().GetProperties();
            //foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            //{
            //    MessageBox.Show(currentProperty.Name);
            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (Regex.IsMatch(host.Text, @"^(?=.*[^\.]$)((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.?){4}$")
                && Regex.IsMatch(hport.Text, regport) && Regex.IsMatch(mport.Text, regport) && Regex.IsMatch(sport.Text, regport))
                {
                    s.host = host.Text;
                    s.prefix = prefix.Text;
                    s.httpPort = int.Parse(hport.Text);
                    s.MqttPort = int.Parse(mport.Text);
                    s.ServerPort = int.Parse(sport.Text);
                    s.Save();
                }
                else
                {
                    MessageBox.Show("Please insert valid ip and port");
                    e.Cancel = true;
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show(ea.ToString());
            }
        }

        private void port_KeyDown(object sender, KeyEventArgs e)
        {
            //KeysConverter kc = new KeysConverter();
            //e.Handled = !char.IsDigit(e.Key) && !char.IsControl(e.KeyChar);
        }
    }
}
