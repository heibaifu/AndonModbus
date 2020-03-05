using AndonModbus.Model;
using Modbus.Device;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace AndonModbus.Utils
{
    class Field
    {

        public static Dictionary<int, string> btnType = new Dictionary<int, string> {
            {1, "NotGood"}, {2, "Repair"}, {3, "Downtime"}, {4, "NGHistory" }, { 5, "MACHINE"}, {6, "SHIFT"}, {7, "RESET"}
        };
        public static List<Machine> machine = new List<Machine>();
        public static List<Machine> mdown = new List<Machine>();
        public static List<Machine> statind = new List<Machine>();
        public static List<Buttons> button = new List<Buttons>();
        public static List<Gateway> gateway = new List<Gateway>();
        public static Shift shift = new Shift("0", "", "", "", "", true, false);
        public static Dictionary<string, Problem> downtimel = new Dictionary<string, Problem>();
        public static Dictionary<string, Problem> NotGoodl = new Dictionary<string, Problem>();
        public static Dictionary<string, Part> part = new Dictionary<string, Part>();
        public static Random r = new Random();
        public static string defstatus = "", secstatus = "", oldshift = "0";
        public static System.Windows.Controls.TextBlock status = null;
        public static Brush black;
        public static Brush red;
        public static Brush red2;
        public static Brush white;
        public static List<string> payload = new List<string>();
        public static List<string> payload2 = new List<string>();
        public static WEB web = null;
        public static Helper help = null;
        public static BackgroundWorker bw = new BackgroundWorker();
        public static MQTT mqtt = null;
        public static WEBS server = null;
        public static bool enddata = false, getData = true, dataLoaded = false, debug=false, derror = false, dreset = false;

        public static void setStatus(string text, bool error)
        {
            Application.Current.Dispatcher.Invoke(() => {
                status.Foreground = error ? red : white;
                status.Text = text;
            });
        }

        public static void wData(int mode, Machine m)
        {
            string[] d = new string[] { };
            JToken jt = JToken.Parse("{}");
            switch (mode)
            {
                case 5:
                    d = new string[] { m.id.ToString(), m.id_line, shift.id, part != null ? m.part.id : "0", m.act.ToString() };
                    jt = JToken.FromObject(m.getData());
                    break;
                case 3:
                    d = new string[] { m.iddt, shift.id, part != null ? m.part.id : "0", "1", m.id.ToString(), DateTime.Now.ToString("yyy/MM/dd HH:mm:s"), (m.downtime ? "true" : "false") };
                    jt["id"] = m.id;
                    jt["dtname"] = m.ldname;
                    jt["isDown"] = m.downtime;
                    jt["otr"] = m.otr;
                    jt["per"] = m.per;
                    jt["qr"] = m.qr;
                    jt["oee"] = m.oee;
                    jt["dtotal"] = m.tdown;
                    jt["dnow"] = m.ldown;
                    jt["eff"] = m.eff;
                    break;
                default:
                    break;
            }

            JObject jo = new JObject();
            jo["mode"] = mode;
            jo["data"] = JToken.FromObject(d);
            payload.Add(jo.ToString());
            if (mqtt.isConnected)
            {
                jt["mode"] = mode;
                payload2.Add(jt.ToString());
            }
            File.WriteAllLines("cache.zc", payload.ToArray());
        }

        public static void sendData(object sender, DoWorkEventArgs e)
        {
            while (payload.Count > 0 || payload2.Count > 0)
            {
                if (payload.Count > 0 && web.postData("api/indata", payload[0]))
                {
                    payload.RemoveAt(0);
                    File.WriteAllLines("cache.zc", payload.ToArray());
                }
                if (mqtt.isConnected && payload2.Count > 0)
                {
                    mqtt.Send(payload2[0]);
                    payload2.RemoveAt(0);
                }
            }
            if (payload.Count > 0)
            {
                RunData();
            }
        }

        public static void RunData()
        {
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
            }
        }

        public static void resetGateway()
        {
            getData = false;
            foreach (Gateway g in gateway)
            {
                try
                {
                    g.master.WriteSingleRegister(1, 16, 0);
                    //MessageBox.Show("ok "+g.name);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            getData = true;
        }

        public static void StartGateway()
        {
            foreach (Gateway g in gateway)
            {
                g.Start();
            }
        }

        public static bool StopGateway(int index=0)
        {
            enddata = true;
            if (gateway.Count > 0)
            {
                for (int i = 0; (!gateway[index].poll.IsBusy || (i < 5)); i++)
                {
                    Thread.Sleep(1000);
                }
                if (!gateway[index].poll.IsBusy)
                {
                    foreach (Gateway g in gateway)
                    {
                        if (g.poll.IsBusy)
                        {
                            StopGateway(g.index);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            enddata = false;
            return true;
        }
    }
}
