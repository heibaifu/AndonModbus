using AndonModbus.Utils;
using AndonServer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AndonServer.Utils
{
    class WEB
    {
        WebClient wc = new WebClient();
        public string Error = "";
        public string HOST = "";
        int actc = 0;
        public WEB()
        {
            HOST = "http://" + Properties.Settings.Default.host + ":" + Properties.Settings.Default.httpPort.ToString() + "/";
        }

        public async Task<string> getData(bool post, string url, string data = "")
        {
            return await getData2(post, HOST + url, data);
        }

        public async Task<string> getData2(bool post, string url, string data = "")
        {
            string r = null;
            try
            {
                if (post)
                {
                    wc.Headers.Add("Content-Type: application/json");
                    r = await wc.UploadStringTaskAsync(url, data);
                }
                else
                {
                    r = await wc.DownloadStringTaskAsync(new Uri(url));
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                //Helper.Msg(Error);
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
                MessageBox.Show(Error);
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
                //Helper.Msg("cannot convert");
            }
            return null;
        }

        public async Task<bool> GetAllData()
        {
            try
            {
                string data = await getData(false, "?cmd=gdata");
                if (data == null)
                {
                    return false;
                }
                JArray core = JArray.Parse(data);
                if (core != null)
                {
                    for (int e = 0; e < core.Count; e++)
                    {
                        Machine m = new Machine(e);
                        JToken i = core[e];
                        m.id = (int)i["id"];
                        m.index = (int)i["index"];
                        m.dtindex = (int)i["dtindex"];
                        m.act = (int)i["act"];
                        m.idgateway = (int)i["idgateway"];
                        m.ctime = (int)i["ctime"];
                        m.otr = (double)i["otr"];
                        m.per = (double)i["per"];
                        m.qr = (double)i["qr"];
                        m.oee = (double)i["oee"];
                        JArray nglab = (JArray)i["nghLab"];
                        foreach (JToken a in nglab)
                        {
                            m.nghLab.Add(a.ToString());
                        }
                        JArray ngval = (JArray)i["nghVal"];
                        foreach (JToken a in ngval)
                        {
                            int val = 0;
                            int.TryParse(a.ToString(), out val);
                            m.nghVal.Add(val);
                        }
                        JArray dtlab = (JArray)i["dtLab"];
                        foreach (JToken a in dtlab)
                        {
                            m.dtLab.Add(a.ToString());
                        }
                        JArray dtval = (JArray)i["dtVal"];
                        foreach (JToken a in dtval)
                        {
                            int val = 0;
                            //MessageBox.Show(a.ToString());
                            int.TryParse(a.ToString(), out val);
                            m.dtVal.Add(val);
                        }
                        m.downtime2 = (bool)i["downtime"];
                        m.isReset = (bool)i["isReset"];
                        //m.part = (int)i["part"];
                        m.btnid = i["btnid"].ToString();
                        m.serial = i["serial"].ToString();
                        m.port = i["port"].ToString();
                        m.op = i["op"].ToString();
                        m.mname = i["mname"].ToString();
                        m.code = i["code"].ToString();
                        m.plan = i["plan"].ToString();
                        m.bal = i["bal"].ToString();
                        m.type = i["type"].ToString();
                        m.proc = i["proc"].ToString();
                        m.work = i["work"].ToString();
                        m.eff = i["eff"].ToString();
                        m.tdown = i["tdown"].ToString();
                        m.ldname = i["ldname"].ToString();
                        m.ldown = i["ldown"].ToString();
                        m.ng = i["ng"].ToString();
                        m.rep = i["rep"].ToString();
                        m.okrep = i["okrep"].ToString();
                        m.id_line = i["id_line"].ToString();
                        Field.machine.Add(m);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                //Helper.Msg("cannot convert");
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
                return false;
            }
        }
    }
}
