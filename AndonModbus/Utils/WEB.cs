using AndonModbus.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AndonModbus.Utils
{
    class WEB
    {
        WebClient wc = new WebClient();
        public string Error = "";
        public string HOST = "";
        MainWindow home = null;
        //Action action = null;
        Helper h = new Helper();
        Machine m = null;
        int actc = 0;
        Properties.Settings s = Properties.Settings.Default;
        public WEB(MainWindow home)
        {
            //action = new Action(() => {
                
            //});
            this.home = home;
            HOST = "http://" + s.host + ":" + s.httpPort + (s.prefix.Length > 0 ? "/" + s.prefix : "") + "/";
        }

        public async Task<string> getData(bool post, string url, string data = "")
        {
            string r = null;
            try
            {
                if (post)
                {
                    wc.Headers.Add("Content-Type: application/json");
                    r = await wc.UploadStringTaskAsync(HOST + url, data);
                }
                else
                {
                    r = await wc.DownloadStringTaskAsync(new Uri(HOST + url));
                    //MessageBox.Show(r);
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                Field.derror = true;
                Helper.Msg(Error+"(Ex.web1)");
                r = null;
            }
            return r;
        }

        public bool postData(string url, string data = "")
        {
            string r = null;
            try
            {
                wc.Headers.Add("Content-Type: application/json");
                r = wc.UploadString(HOST + url, data);
            }
            catch (Exception e)
            {
                Error = e.Message;
                //MessageBox.Show(Error);
                r = null;
            }
            return true;
        }

        public BitmapFrame getImage(string imgname)
        {
            try
            {
                byte[] img = wc.DownloadData(new Uri(HOST + "/view/rsc/img/" + imgname));
                return BitmapFrame.Create(new MemoryStream(img), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
            return null;
        }

        public JObject Parse(string jstring)
        {
            try
            {
                return JObject.Parse(jstring);
            }
            catch (Exception e)
            {
                Helper.Msg("cannot convert");
            }
            return null;
        }

        public async void getAllData(bool isfReset)
        {
            Field.dreset = isfReset;
            try
            {
                Field.defstatus = "";
                Field.secstatus = "";
                Field.derror = false;
                Field.dataLoaded = false;
                if (Field.mqtt.isConnected)
                {
                    Field.mqtt.Stop();
                }
                foreach (Gateway g in Field.gateway)
                {
                    g.close();
                }
                string data = pJson(await getData(false, "api/getDataMonitor/1/" + (isfReset ? "1" : "0")));
                if (data == null)
                {
                    return;
                }
                JObject core = JObject.Parse(data);
                if (core == null)
                {
                    return;
                }
                JToken shift = core.SelectToken("shift_n");
                if (shift == null)
                {
                    return;
                }
                Field.shift = new Shift(shift["id"].ToString(), shift["name"].ToString(), shift["start"].ToString(),
                    shift["stop"].ToString(), shift["break"].ToString());
                Field.shift.diff = shift["diff"].ToString();
                if (Field.shift.id != Properties.Settings.Default.shiftn)
                {
                    //Field.oldshift = core.GetValue("oldshift.id_shift").ToString();
                    Field.resetGateway();
                    Properties.Settings.Default.shiftn = Field.shift.id;
                    Properties.Settings.Default.Save();
                }
                JArray btn = (JArray)core["button"];
                Field.button.Clear();
                foreach (JToken i in btn)
                {
                    Field.button.Add(new Buttons(i["id"].ToString(), i["serial"].ToString(), i.Value<int>("type")));
                }
                JArray dtl = (JArray)core["dt_list"];
                Field.downtimel.Clear();
                foreach (JToken i in dtl)
                {
                    string no = i["no"].ToString();
                    Field.downtimel.Add(no, new Problem(i["id"].ToString(), no, i["name"].ToString(), i["allowed"].ToString()));
                }
                JArray ngl = (JArray)core["ng_list"];
                Field.NotGoodl.Clear();
                foreach (JToken i in ngl)
                {
                    string no = i["number"].ToString();
                    Field.NotGoodl.Add(no, new Problem(i["id"].ToString(), no, i["name"].ToString(), i["allowed"].ToString()));
                }
                JArray part = (JArray)core["part"];
                Field.part.Clear();
                foreach (JToken i in part)
                {
                    Field.part.Add(i["id"].ToString(), new Part(i["id"].ToString(), i["name"].ToString(), i["image"].ToString()));
                }
                JArray mcn = (JArray)core["machine"];
                Field.machine.Clear();
                Field.mdown.Clear();
                string diff = "0";
                if (Field.shift.diff.Contains(":"))
                {
                    diff = h.dtmin(Field.shift.diff).ToString();
                }
                for (int i = 0; i < mcn.Count; i++)
                {
                    Machine m = new Machine(home, i);
                    m.idgateway = 1;
                    m.id_line = mcn[i]["id_line"].ToString();
                    m.id = int.Parse(mcn[i]["id"].ToString());
                    m.mname = mcn[i]["name"].ToString();
                    m.code = mcn[i]["code"].ToString();
                    m.proc = mcn[i]["process"].ToString();
                    m.type = mcn[i]["type"].ToString();
                    string idpart = mcn[i]["id_part"].ToString();
                    m.part = Field.part.ContainsKey(idpart) ? Field.part[idpart] : null;
                    int ctime = 0;
                    m.ctime = int.TryParse(mcn[i]["ct"].ToString(), out ctime) ? ctime : 0;
                    int act = 0;
                    m.act = int.TryParse(mcn[i]["act"].ToString(), out act) ? act : 0;
                    actc = act;
                    bool isReset = true;
                    m.isReset = bool.TryParse(mcn[i]["isReset"].ToString(), out isReset) ? isReset : true;
                    m.btnid = mcn[i]["button_id"].ToString();
                    m.idgateway = (int)mcn[i]["g_id"];
                    m.serial = mcn[i]["serial"].ToString();
                    m.port = mcn[i]["port"].ToString();
                    m.work = diff;
                    Field.machine.Add(m);
                    this.m = m;
                    h.mHitung(5, m, actc);
                }
                JArray dth = (JArray)core["downtime"];
                for (int i = 0; i < dth.Count; i++)
                {
                    Machine m = Field.machine.Find(x => x.id.ToString() == dth[i]["id_engine"].ToString());
                    if (m != null)
                    {
                        if (Field.downtimel.ContainsKey(dth[i]["id_item"].ToString()))
                        {
                            Problem p = Field.downtimel[dth[i]["id_item"].ToString()];
                            m.dtLab.Add(p.name);
                            int val = 0;
                            int.TryParse(dth[i]["jml"].ToString(), out val);
                            m.dtVal.Add(val);
                            string dif = dth[i]["diff"].ToString();
                            if (dif != "true")
                            {
                                string[] wd = dif.Split(':');
                                m.hour2 = int.Parse(wd[0]);
                                m.minute2 = int.Parse(wd[1]);
                                m.sec2 = int.Parse(wd[2]);
                                m.iddt = p.id;
                                m.ldname = p.name;
                                m.DownTime(true, false);
                            }
                        }
                    }
                }
                JArray gateway = (JArray)core["gateway"];
                Field.gateway.Clear();
                for (int i = 0; i < gateway.Count; i++)
                {
                    JToken g = gateway[i];
                    Gateway gway = new Gateway((int)g["id"], g["name"].ToString(), g["serial"].ToString());
                    gway.index = i;
                    Field.gateway.Add(gway);
                    List<Machine> m = Field.machine.FindAll(x => x.idgateway == gway.id);
                    foreach (Machine ma in m)
                    {
                        ma.gway = gway;
                    }
                }
                Field.server.Start();
                if (Field.machine.Count > 0)
                {
                    Field.defstatus = String.Format("Running with shift {0} from {1} to {2} ({3}min)",
                    Field.shift.name, Field.shift.start, Field.shift.stop, diff);
                    Field.setStatus(Field.defstatus, false);
                    Field.StartGateway();
                }
                else
                {
                    Helper.Msg("No machine found");
                }
                //Helper.Msg("",Field.machine.Count + ":" + Field.button.Count.ToString() + ":" + Field.downtimel.Count.ToString() + ":" + Field.NotGoodl.Count.ToString() + ":" + Field.shift.name);
            }
            catch (Exception e)
            {
                Field.derror = true;
                //MessageBox.Show(e.ToString());
                Helper.Msg(e.Message + "(Ex.web2)");
            }
            Field.dataLoaded = true;
        }

        private string pJson(string data)
        {
            if (data != null)
            {
                JObject _jo = JObject.Parse(data);
                if ((bool)_jo.GetValue("status"))
                {
                    return _jo.GetValue("data").ToString();
                }
                else
                {
                    int code = (int)_jo.GetValue("code");
                    if (code == 2)
                    {
                        JToken shift = _jo.GetValue("next");
                        if (shift.ToString() == "0")
                        {
                            Helper.Msg("No shift available", true);
                            Field.shift = new Shift("0", "", "", "", "", true, false);
                            return null;
                        }
                        Helper.Msg("No shift for this time, next shift at " + _jo.SelectToken("next.start"), true);
                        Field.shift = new Shift(shift["id"].ToString(), shift["name"].ToString(), shift["start"].ToString(),
                            shift["stop"].ToString(), shift["break"].ToString(), true);
                    }
                    else
                    {
                        Helper.Msg("Something wrong, please check data on server!", true);
                    }
                }
            }
            else
            {
                //loading(true, help.Error + ", try to get after 5s");
                //Retry(false);
            }
            return null;
        }
    }
}
