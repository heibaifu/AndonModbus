using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AndonServer.View
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
            hport.Text = s.httpPort.ToString();
            mport.Text = s.mqttPort.ToString();
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
                && Regex.IsMatch(hport.Text, regport) && Regex.IsMatch(mport.Text, regport))
                {
                    s.host = host.Text;
                    s.httpPort = int.Parse(hport.Text);
                    s.mqttPort = int.Parse(mport.Text);
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
