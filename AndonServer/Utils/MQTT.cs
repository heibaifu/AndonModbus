using AndonModbus.Utils;
using AndonServer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AndonServer.Utils
{
    class MQTT
    {
        public MqttClient mqtt = null;
        BackgroundWorker bw = new BackgroundWorker();
        List<string> data = new List<string>();
        int retry = 0;
        public MQTT()
        {
            bw.DoWork += RunData;
            try
            {
                mqtt = new MqttClient(Properties.Settings.Default.host, Properties.Settings.Default.mqttPort, false, null, null, MqttSslProtocols.None);
                mqtt.MqttMsgPublishReceived += MsgRec;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void RunData(object sender, DoWorkEventArgs e)
        {
            Machine m = null;
            while (data.Count > 0)
            {
                JToken jt = JToken.Parse(data[0]);
                if (jt != null)
                {
                    switch ((int)jt["mode"])
                    {
                        case 3:
                            m = Field.machine.Find(x => x.id == (int)jt["id"]);
                            if (m != null)
                            {
                                m.ldname = jt["dtname"].ToString();
                                m.otr = (double)jt["otr"];
                                m.per = (double)jt["per"];
                                m.qr = (double)jt["qr"];
                                m.oee = (double)jt["oee"];
                                m.tdown = jt["dtotal"].ToString();
                                m.ldown = jt["dnow"].ToString();
                                m.eff = jt["eff"].ToString();
                                m.DownTime((bool)jt["isDown"]);
                                m.UpdateUI();
                            }
                            break;
                        case 5:
                            m = Field.machine.Find(x => x.id == (int)jt["id"]);
                            if (m != null)
                            {
                                m.act = (int)jt["act"];
                                m.otr = (double)jt["otr"];
                                m.per = (double)jt["per"];
                                m.qr = (double)jt["qr"];
                                m.oee = (double)jt["oee"];
                                m.plan = jt["plan"].ToString();
                                m.bal = jt["bal"].ToString();
                                m.eff = jt["eff"].ToString();
                                m.UpdateUI();
                            }
                            break;
                        default:
                            break;
                    }
                }
                data.RemoveAt(0);
            }
            if (data.Count > 0)
            {
                run();
            }
        }

        private void run()
        {
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
            }
        }

        public async Task<bool> Connect()
        {
            retry++;
            if (retry > 2)
            {
                retry = 0;
                return false;
            }
            try
            {
                await Task.Run(() =>
                {
                    string clientId = "client" + Field.r.Next(20000);
                    mqtt.Connect(clientId);
                    if (mqtt.IsConnected)
                    {
                        retry = 0;
                        mqtt.Subscribe(new string[] { "m0.01t" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                    }
                });
            }
            catch (Exception e)
            {
                await Connect();
            }
            return mqtt.IsConnected;
        }

        private void MsgRec(object sender, MqttMsgPublishEventArgs e)
        {
            data.Add(Encoding.UTF8.GetString(e.Message));
            run();
        }
    }
}
