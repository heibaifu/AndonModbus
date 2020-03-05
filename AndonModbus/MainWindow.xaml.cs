using AndonModbus.Model;
using AndonModbus.Utils;
using AndonModbus.View;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AndonModbus
{
    public partial class MainWindow : Window
    {
        int a = 0, b = 0;
        public int idnow = 0;
        DispatcherTimer slide = new DispatcherTimer();
        Random r = new Random();
        Random r2 = new Random();
        Helper h = new Helper();
        Properties.Settings s = Properties.Settings.Default;
        //SerialPort gateway = new SerialPort("COM5");
        //Machine m = null;
        WEB data = null;
        public MainWindow()
        {
            InitializeComponent();
            Field.status = status;
            data = new WEB(this);
            Field.web = data;
            per.LabelFormatter = x => x + "%";
            otr.LabelFormatter = x => x + "%";
            qr.LabelFormatter = x => x + "%";
            oee.LabelFormatter = x => x + "%";
            try
            {
                slide.Interval = TimeSpan.FromSeconds(1);
                slide.Tick += ExSlide;
                slide.Start();
                data.getAllData(false);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        int ch = 0;
        int ch2 = 0;
        bool defst = true;
        private void ExSlide(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            string timel = time.ToLongTimeString();
            //lTgl.Text = time.ToShortDateString();
            jam.Content = timel;
            if (ch2 == 3)
            {
                if (Field.derror)
                {
                    Field.defstatus = "Reloading data ...";
                    data.getAllData(Field.dreset);
                }
                Field.setStatus(defst ? Field.defstatus : Field.secstatus, !defst);
                defst = !defst;
                ch2 = 0;
            }
            ch2++;
            if (Field.dataLoaded)
            {
                if (Field.shift.next && Field.shift.allow)
                {
                    if (timel == Field.shift.start)
                    {
                        data.getAllData(true);
                    }
                }
                else if (Field.shift.allow)
                {
                    if (Field.debug || timel == Field.shift.stop)
                    {
                        Field.getData = false;
                        Field.setStatus("Reload data ...", false);
                        Application.Current.Dispatcher.Invoke(()=> {
                            Field.web.getAllData(true);
                        });
                        Field.getData = true;
                        return;
                    }
                }

                if (Field.machine.Count < 1)
                {
                    return;
                }
                ch++;
                if (ch == 8)
                {
                    if (a >= (Field.mdown.Count < 1 ? (Field.statind.Count < 1 ? Field.machine.Count : Field.statind.Count) : Field.mdown.Count))
                    {
                        a = 0;
                    }
                    changeDisplay(a);
                    a++;
                    ch = 0;
                }
            }
        }

        private void notgood_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void title_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //MessageBox.Show("Monitoring " + (await stopData() ? "can" : "can't") + " stoped " + poll.IsBusy);
            //MessageBox.Show(Field.server);
            Field.debug = true;
        }

        public void changeDisplay(int index)
        {
            if (index < (Field.mdown.Count < 1 ? (Field.statind.Count < 1 ? Field.machine.Count : Field.statind.Count) : Field.mdown.Count))
            {
                Machine m = (Field.mdown.Count < 1 ? (Field.statind.Count < 1 ? Field.machine : Field.statind) : Field.mdown)[index];
                idnow = m.index;
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
            else
            {
                changeDisplay(0);
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //MessageBox.Show(e.ToString());
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new Settings().ShowDialog();
        }

        public void setDowntime(Machine m)
        {
            dt.Content = m.tdown;
            dtname.Content = m.ldname;
            dt2.Content = m.ldown;
        }
    }
}
