using AndonModbus.Utils;
using AndonServer.Models;
using System;
using System.Windows;
using System.Windows.Threading;

namespace AndonServer
{
    public partial class DetailUI : Window
    {
        Machine m = null;
        public DetailUI(Machine m)
        {
            InitializeComponent();
            m.detail = this;
            this.m = m;
            changeDisplay();
        }

        public void changeDisplay()
        {
            machineCode.Content = m.code;
            plan.Content = m.plan;
            act.Content = m.act;
            name.Content = m.mname;
            bal.Content = m.bal;
            type.Content = m.type;
            proc.Content = m.proc;
            ct.Content = m.ctime;
            work.Content = m.work;
            eff.Content = m.eff;
            repair.Content = m.rep;
            okrepair.Content = m.okrep;
            dt.Content = m.tdown;
            dtname.Content = m.ldname;
            dt2.Content = m.ldown;
            ng.Content = m.ng;
            per.Value = m.per;
            otr.Value = m.otr;
            qr.Value = m.qr;
            oee.Value = m.oee;
            downtime.AxisX[0].Labels = m.dtLab;
            downtime.Series[0].Values = m.dtVal;
            notgood.AxisX[0].Labels = m.nghLab;
            notgood.Series[0].Values = m.nghVal;
            if (m.part != null)
            {
                partname.Content = m.part.name;
                partImage.Source = m.part.getImg();
            }
            if (!m.downtime)
            {
                parent.Background = Field.black;
            }
            else
            {
                parent.Background = Field.red2;
            }
        }

        public void setDowntime(Machine m)
        {
            dt.Content = m.tdown;
            dtname.Content = m.ldname;
            dt2.Content = m.ldown;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m.detail = null;
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }
    }
}
