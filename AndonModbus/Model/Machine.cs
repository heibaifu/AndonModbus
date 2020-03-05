using AndonModbus.Utils;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace AndonModbus.Model
{
    public class Machine
    {
        DispatcherTimer tm = new DispatcherTimer();
        private MainWindow d = null;
        public int id = 0, index = -1, dtindex = -1, act = 0, idgateway = 0,
            ctime = 0, hour = 0, minute = 0, sec = 0, hour2 = 0, minute2 = 0, sec2 = 0;
        public double otr, per, qr, oee;
        public List<string> nghLab = new List<string>(), dtLab = new List<string>();
        public ChartValues<double> nghVal = new ChartValues<double>(), dtVal = new ChartValues<double>();
        public bool downtime = false, isReset = false;
        Random r = new Random();
        public Gateway gway = null;
        Action action = null;
        Action ac2 = null;
        public Part part = null;
        public string btnid = "", serial = "", port = "", op = "-", mname = "-", code = "-",
            plan = "9000", bal = "0", type = "-", proc = "-",
            work = "0", eff = "0", tdown = "00:00:00", ldname = "-", ldown = "00:00:00",
            ng = "0", rep = "0", okrep = "0", id_line = "0", iddt = "", idng = "";

        public Machine(MainWindow d, int index)
        {
            this.d = d;
            this.index = index;
            action = new Action(() => {
                d.changeDisplay(index);
            });
            ac2 = new Action(() => {
                d.setDowntime(this);
            });
            tm.Interval = TimeSpan.FromSeconds(1);
            tm.Tick += Tm_Tick;
            //for (int i = 0; i < 10; i++)
            //{
            //    dtLab.Add(i.ToString());
            //    dtVal.Add(Field.r.Next(10));
            //    nghLab.Add(i.ToString());
            //    nghVal.Add(Field.r.Next(10));
            //}
        }

        public bool AddAct(int act)
        {
            bool added = false;
            if (act > this.act)
            {
                this.act = act;
                added = true;
            }
            else if(act < this.act)
            {
                if (gway != null)
                {
                    Field.getData = false;
                    int pr = -1;
                    if (int.TryParse(port, out pr))
                    {
                        pr = pr - 1;
                        gway.master.WriteSingleRegister(1, (ushort)pr, (ushort)this.act);
                    }
                    Field.getData = true;
                }
            }
            return added;
        }

        public void UpdateUI()
        {
            if (d.idnow == index)
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }

        public void DownTime(bool start, bool rtime=true)
        {
            if (start && !downtime)
            {
                if (rtime)
                {
                    hour2 = 0; minute2 = 0; sec2 = 0;
                }
                string[] ts = tdown.Split(':');
                hour = int.Parse(ts[0]);
                minute = int.Parse(ts[1]);
                sec = int.Parse(ts[2]);
                tm.Start();
                downtime = true;
                if (!Field.mdown.Contains(this))
                {
                    Field.mdown.Add(this);
                    Application.Current.Dispatcher.Invoke(action);
                }
            }
            else if (!start)
            {
                tm.Stop();
                downtime = false;
                Field.mdown.Remove(this);
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
            if (d.idnow == index)
            {
                Application.Current.Dispatcher.Invoke(ac2);
            }
        }

        void UUI(int index)
        {
            d.changeDisplay(index);
        }

        public void addData(int mode)
        {
            Field.wData(mode, this);
            Field.RunData();
        }

        public data getData()
        {
            data d = new data();
            d.id = id;
            d.index = index;
            d.dtindex = dtindex;
            d.act = act;
            d.otr = otr;
            d.per = per;
            d.qr = qr;
            d.oee = oee;
            d.port = port;
            d.plan = plan;
            d.bal = bal;
            d.eff = eff;
            return d;
        }

        public class data
        {
            public int id, index, dtindex, act;
            public double otr, per, qr, oee;
            public string port, plan, bal, eff;
        }
    }
}
