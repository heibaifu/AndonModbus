using AndonModbus.Utils;
using AndonServer.Models;
using AndonServer.Utils;
using AndonServer.View;
using LiveCharts.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AndonServer
{
    /// <summary>
    /// Interaction logic for Server.xaml
    /// </summary>
    public partial class MainUI : Window
    {
        MQTT mqtt = new MQTT();
        int delay = 3, dcount=0;
        bool allow = true;
        Grid mactive = null;
        public MainUI()
        {
            InitializeComponent();
            try
            {
                getData();
                mqtt.mqtt.ConnectionClosed += C_Close;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void C_Close(object sender, EventArgs e)
        {
            try
            {
                if (allow)
                {
                    if (delay < 59)
                    {
                        delay = delay + 2;
                    }
                    dcount = delay;
                    Application.Current.Dispatcher.Invoke(() => {
                        status.Text = "Filed getting data, retrying after " + delay + "s";
                    });
                    Field.time.Tick += MQTRY;
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show(ea.ToString());
            }
        }

        private async void getData()
        {
            Field.time.Tick -= MQTRY;
            gdata.Children.Clear();
            Field.machine.Clear();
            Application.Current.Dispatcher.Invoke(()=> {
                nomc.Visibility = Visibility.Hidden;
            });
            status.Text = "Loading data ...";
            if (await Field.web.GetAllData())
            {
                try
                {
                    DataTemplate dt = FindResource("duka") as DataTemplate;
                    Thickness bpd = new Thickness(0, 3, 3, 0);
                    int col = 0;
                    int row = 0;
                    foreach (Machine m in Field.machine)
                    {
                        Border c1 = new Border();
                        c1.MinHeight = 200;
                        c1.MinWidth = 100;
                        c1.Padding = bpd;
                        c1.Cursor = System.Windows.Input.Cursors.Hand;
                        Grid g = (Grid)dt.LoadContent();
                        g.Tag = m.index;
                        g.MouseLeftButtonDown += Field.IClick;
                        Label title = (Label)((Border)g.Children[0]).Child;
                        Label plan = (Label)((Border)g.Children[2]).Child;
                        Label act = (Label)((Border)g.Children[4]).Child;
                        title.Content = m.mname + " (" + m.code + ")";
                        plan.Content = m.plan;
                        act.Content = m.act;
                        m.lp = plan;
                        m.la = act;
                        Gauge g1 = (Gauge)((Border)g.Children[5]).Child;
                        g1.LabelFormatter = x => x + "%";
                        c1.Child = g;
                        gdata.Children.Add(c1);
                        Grid.SetColumn(c1, col);
                        Grid.SetRow(c1, row);
                        g1.Value = m.oee;
                        m.goee = g1;
                        m.g = g;
                        if (m.downtime2)
                        {
                            m.DownTime(true);
                        }
                        if (col == 4)
                        {
                            col = -1;
                            gdata.RowDefinitions.Add(new RowDefinition());
                            row++;
                        }
                        col++;
                    }
                    RTConnect();
                    if (Field.machine.Count < 1)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            nomc.Visibility = Visibility.Visible;
                        });
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                C_Close(null, null);
            }
        }

        private async void RTConnect()
        {
            status.Text = "Connecting to client ...";
            if (await mqtt.Connect())
            {
                status.Text = "Running";
            }
            else
            {
                C_Close(null, null);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mqtt.mqtt.IsConnected)
            {
                mqtt.mqtt.Disconnect();
            }
            allow = false;
        }

        private void MenuClick(object sender, RoutedEventArgs e)
        {
            if (mactive != null)
            {
                MessageBox.Show(mactive.Tag.ToString());
            }
        }

        private void MenuOpen(object sender, ContextMenuEventArgs e)
        {
            mactive = (Grid)sender;
        }

        private void MenuClose(object sender, ContextMenuEventArgs e)
        {
            mactive = null;
        }

        private async void MenuCheck(object sender, RoutedEventArgs e)
        {
            MenuItem m = (MenuItem)sender;
            int ind = -1;
            if (!int.TryParse(mactive.Tag.ToString(), out ind))
            {
                return;
            }
            if (await Field.web.getData2(false, "http://" + Properties.Settings.Default.host + ":8081/?cmd="+(m.IsChecked ? "stop" : "start") +"&id=" + Field.machine[ind].id) != "true")
            {
                m.IsChecked = false;
            }
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            new Settings().ShowDialog();
        }

        private void MQTRY(object sender, EventArgs e)
        {
            dcount--;
            if (dcount == 0)
            {
                getData();
            }
            else
            {
                status.Text = "Reloading after " + dcount + "s";
            }
        }
    }
}
