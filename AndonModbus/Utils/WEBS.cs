using AndonModbus.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Windows;
using TinyWeb;

namespace AndonModbus.Utils
{
    class WEBS
    {
        WebServer ws = new WebServer();
        public WEBS()
        {
            ws.ProcessRequest += Ws_ProcessRequest;
            ws.EndPoint = new IPEndPoint(IPAddress.Any, Properties.Settings.Default.ServerPort);
        }

        public void Start()
        {
            ws.IsStarted = true;
        }

        public void Stop()
        {
            ws.IsStarted = false;
        }

        private void Ws_ProcessRequest(object sender, ProcessRequestEventArgs e)
        {
            if (Field.dataLoaded)
            {
                try
                {
                    //r.ContentType = "text/html";
                    HttpRequest req = e.Request;
                    HttpResponse res = e.Response;
                    string cmd = "", data = "";
                    req.TryGetFirstQueryValue("cmd", out cmd);
                    req.TryGetFirstQueryValue("data", out data);
                    e.Response.IsKeepAlive = false;
                    switch (cmd)
                    {
                        case "btn":
                            Buttons b = Field.button.Find(x => x.code == data);
                            if (b != null)
                            {
                                Machine m = Field.machine.Find(x => x.btnid.Contains("," + b.id + ","));
                                if (m != null)
                                {
                                    string no = "";
                                    req.TryGetFirstQueryValue("no", out no);
                                    switch (b.type)
                                    {
                                        case 1: //NotGood
                                            if (no != null && Field.NotGoodl.ContainsKey(no))
                                            {
                                                MessageBox.Show("This is ng button for machine id " + m.id + " and ng is " + Field.NotGoodl[no].name);
                                            }
                                            break;
                                        case 2: //Repair
                                            MessageBox.Show("This is repair button for machine id " + m.id + " and repair is " + no);
                                            break;
                                        case 3: //Downtime
                                            if (no != null)
                                            {
                                                if (no != "12" && !m.downtime && Field.downtimel.ContainsKey(no))
                                                {
                                                    string dtname = Field.downtimel[no].name;
                                                    int index = -1;
                                                    if ((index = m.dtLab.FindIndex(x => x == dtname)) == -1)
                                                    {
                                                        m.dtLab.Add(dtname);
                                                        m.dtVal.Add(1);
                                                        index = m.dtLab.Count - 1;
                                                    }
                                                    else
                                                    {
                                                        m.dtVal[index]++;
                                                    }
                                                    m.ldname = dtname;
                                                    m.iddt = no;
                                                    m.DownTime(true);
                                                    Field.help.mHitung(3, m, m.act);
                                                    //MessageBox.Show("This is downtime button for machine id " + m.id + " and downtime is " + Field.downtimel[no].name);
                                                }
                                                else if (no == "12")
                                                {
                                                    m.DownTime(false);
                                                    Field.help.mHitung(3, m, m.act);
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            break;
                        case "shift":
                            break;
                        case "reset":
                            //MessageBox.Show(await Field.web.getData(true, "api/indata", "{\"mode\":-1, \"data\":\"\"}"));
                            //Field.resetGateway();
                            Field.getData = false;
                            e.Response.Write("Reseting");
                            Field.resetGateway();
                            Field.setStatus("Reload data ...", false);
                            Field.web.getAllData(true);
                            Field.getData = true;
                            break;
                        case "gdata":
                            if (!Field.mqtt.isConnected)
                            {
                                Field.mqtt.Start();
                            }
                            if (Field.mqtt.isConnected)
                            {
                                JArray ja = new JArray();
                                foreach (Machine m in Field.machine)
                                {
                                    ja.Add(JToken.FromObject(m));
                                }
                                e.Response.Write(ja.ToString());
                            }
                            break;
                        case "stop":
                            string no2 = "";
                            req.TryGetFirstQueryValue("id", out no2);
                            Machine m2 = Field.machine.Find(x => x.id.ToString() == no2);
                            if (m2 != null && !Field.statind.Contains(m2))
                            {
                                Field.statind.Add(m2);
                                e.Response.Write("true");
                                return;
                            }
                            e.Response.Write("false");
                            break;
                        case "start":
                            string no3 = "";
                            req.TryGetFirstQueryValue("id", out no3);
                            if (no3 == "all")
                            {
                                Field.statind.Clear();
                                e.Response.Write("true");
                                return;
                            }
                            else
                            {
                                int m3 = Field.statind.FindIndex(x => x.id.ToString() == no3);
                                if (m3 > -1)
                                {
                                    Field.statind.RemoveAt(m3);
                                    e.Response.Write("true");
                                    return;
                                }
                            }
                            e.Response.Write("false");
                            break;
                        default:
                            e.Response.ContentType = "text/html";
                            e.Response.Write("<a href=\"?cmd=reset\"><button>Reset</button></a>");
                            break;
                    }
                }
                catch (Exception ea)
                {
                    MessageBox.Show(ea.Message + "\n" + ea.StackTrace);
                }
            }
        }
    }
}
