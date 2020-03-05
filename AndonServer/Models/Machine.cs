using AndonModbus.Utils;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AndonServer.Models
{
    public class Machine
    {
        public DispatcherTimer tmm = new DispatcherTimer();
        int hour = 0, minute = 0, sec = 0,
            hour2 = 0, minute2 = 0, sec2 = 0;
        public Label lp=null, la=null;
        public Gauge goee = null;
        public Grid g = null;
        public DetailUI detail = null;
        public int id = 0, index = -1, dtindex = -1, act = 0, idgateway = 0, ctime = 0;
        public double otr, per, qr, oee;
        public List<string> nghLab = new List<string>(), dtLab = new List<string>();
        public ChartValues<int> nghVal = new ChartValues<int>(), dtVal = new ChartValues<int>();
        public bool downtime = false, downtime2 = false, isReset = false;
        Random r = new Random();
        Action action = null;
        public Part part = null;
        public string btnid = "", serial = "", port = "", op = "-", mname = "-", code = "-",
            plan = "9000", bal = "0", type = "-", proc = "-",
            work = "120", eff = "0", tdown = "00:00:00", ldname = "-", ldown = "00:00:00",
            ng = "0", rep = "0", okrep = "0", id_line = "0";

        public Machine(int index)
        {
            tmm.Interval = TimeSpan.FromSeconds(1);
            tmm.Tick += Tm_Tick;
            action = new Action(() =>
            {
                lp.Content = plan;
                la.Content = act;
                goee.Value = oee;
                if (detail != null)
                {
                    detail.changeDisplay();
                }
            });
        }

        public void UpdateUI()
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        public void DownTime(bool start)
        {
            try
            {
                if (start && !downtime)
                {
                    string[] ts = tdown.Split(':');
                    hour = int.Parse(ts[0]);
                    minute = int.Parse(ts[1]);
                    sec = int.Parse(ts[2]);
                    string[] ts2 = ldown.Split(':');
                    hour2 = int.Parse(ts2[0]);
                    minute2 = int.Parse(ts2[1]);
                    sec2 = int.Parse(ts2[2]);
                    tmm.Start();
                    downtime = true;
                    Application.Current.Dispatcher.Invoke(() => {
                        g.Background = Field.red;
                    });
                }
                else if (!start)
                {
                    tmm.Stop();
                    downtime = false;
                    Application.Current.Dispatcher.Invoke(() => {
                        g.Background = Field.black;
                    });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void Tm_Tick(object sender, EventArgs e)
        {
            if (sec < 59)
            {
                sec++;
                sec2++;
            }
            else if (minute < 59)
            {
                minute++;
                minute2++;
                sec = 0;
                sec2 = 0;
            }
            else
            {
                hour++;
                minute = 0;
                sec = 0;
                hour2++;
                minute2 = 0;
                sec2 = 0;
            }
            tdown = hour.ToString("00") + ":" + minute.ToString("00") + ":" + sec.ToString("00");
            ldown = hour2.ToString("00") + ":" + minute2.ToString("00") + ":" + sec2.ToString("00");
            if (detail != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    detail.setDowntime(this);
                });
            }
        }
    }
}
