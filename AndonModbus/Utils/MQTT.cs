using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Threading.Tasks;

namespace AndonModbus.Utils
{
    class MQTT : IMqttApplicationMessageReceivedHandler, IMqttServerClientDisconnectedHandler,
        IMqttServerClientConnectedHandler, IMqttServerStoppedHandler, IMqttServerStartedHandler
    {
        IMqttServer mqtt = null;
        public static MQTT mqt = null;
        IMqttServerOptions opt = null;
        public bool isConnected = false;
        int client = 0;
        string cid = "0.01server", ttopic = "m0.01t", rtopic = "m0.01r";

        public MQTT()
        {
            MqttServerOptionsBuilder prop = new MqttServerOptionsBuilder().WithDefaultEndpointPort(Properties.Settings.Default.MqttPort)
                .WithConnectionValidator(x=> {
                    x.ReasonCode = MqttConnectReasonCode.Success;
                })
                .WithClientId(cid);
            opt = prop.Build();
        }

        public static void init()
        {
            if (mqt == null)
            {
                mqt = new MQTT();
            }
        }

        public void Send(string data)
        {
            mqtt.PublishAsync(ttopic, data);
        }

        public async void Start()
        {
            mqtt = new MqttFactory().CreateMqttServer();
            mqtt.ApplicationMessageReceivedHandler = this;
            mqtt.ClientDisconnectedHandler = this;
            mqtt.ClientConnectedHandler = this;
            mqtt.StartedHandler = this;
            mqtt.StoppedHandler = this;
            await mqtt.StartAsync(opt);
        }

        public async void Stop()
        {
            try
            {
                await mqtt.StopAsync();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }


        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            if (e.ClientId != cid && e.ApplicationMessage.Topic != ttopic)
            {
                Send(e.ApplicationMessage.ConvertPayloadToString());
            }
            return Task.FromResult(true);
        }

        public Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
        {
            client++;
            return Task.FromResult(true);
        }

        public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
        {
            client--;
            if (client < 1)
            {
                mqtt.StopAsync();
            }
            return Task.FromResult(true);
        }

        public Task HandleServerStoppedAsync(EventArgs eventArgs)
        {
            isConnected = false;
            return Task.FromResult(true);
        }

        public Task HandleServerStartedAsync(EventArgs eventArgs)
        {
            isConnected = true;
            return Task.FromResult(true);
        }
    }
}
