using AndonModbus.Utils;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AndonModbus.Model
{
    public class Gateway
    {
        public int id = 0, index = -1;
        public string name = "", serial = "";
        public IModbusSerialMaster master = null;
        SerialPort sport = new SerialPort();
        public BackgroundWorker poll = new BackgroundWorker();
        int actc = 0;
        int retry = 5;
        public Gateway(int id, string name, string serial)
        {
            this.id = id;
            this.name = name;
            this.serial = serial;
            sport.PortName = serial;
            sport.BaudRate = 9600;
            poll.DoWork += getdt;
            master = ModbusSerialMaster.CreateRtu(sport);
            master.Transport.ReadTimeout = 1000;
            master.Transport.Retries = 1;
            master.Transport.WriteTimeout = 1000;
        }

        public void Start()
        {
            Task.Run(()=> {
                while (!sport.IsOpen)
                {
                    try
                    {
                        if (!sport.IsOpen)
                        {
                            sport.Open();
                        }
                        if (sport.IsOpen)
                        {
                            retry = 0;
                            if (!poll.IsBusy)
                            {
                                poll.RunWorkerAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (retry == 1)
                        {
                            Field.secstatus += "Fail " + name + " retrying ";
                        }
                    }
                }
            });
        }

        public void close()
        {
            sport.Close();
        }

        private void getdt(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (Field.enddata || !sport.IsOpen)
                {
                    break;
                }
                if (Field.getData)
                {
                    try
                    {
                        if (sport.IsOpen)
                        {
                            ushort[] d = master.ReadInputRegisters(1, 0, 15);
                            List<Machine> mc = Field.machine.FindAll(x => x.idgateway == id);
                            foreach (Machine i in mc)
                            {
                                int ac = 0;
                                if (int.TryParse(i.port, out ac))
                                {
                                    ac = ac - 1;
                                    if (ac < d.Length)
                                    {
                                        actc = d[ac];
                                        Field.help.mHitung(5, i, actc);
                                        i.UpdateUI();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ea)
                    {
                        //MessageBox.Show(ea.Message + "\n" + ea.StackTrace);
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
