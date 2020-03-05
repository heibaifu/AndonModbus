using System;
using System.Windows;

namespace AndonModbus.Utils
{
    class Helper
    {
        public static void Msg(string msg, bool error=false)
        {
            Field.secstatus = msg.ToUpper();
            //MessageBox.Show(msg.ToUpper());
        }
        public double plan(string jam_kerja, double ct)
        {
            //MessageBox.Show((ct / 60).ToString());
            return (double.Parse(jam_kerja) * (60 / ct));
        }

        public int balance(string ok, string plan)
        {
            return (int.Parse(ok) - int.Parse(plan));
        }

        public float eff(string ok, string plan)
        {
            float pok = ok != "0" ? float.Parse(ok) : 1;
            float pl = plan != "0" ? float.Parse(plan) : 1;
            pl = ((pok / pl) * 100);
            return pl < 0 ? 0 : pl > 100 ? 100 : pl;
        }

        public float png(string ok, string scrap)
        {
            float pok = ok != "0" ? float.Parse(ok) : 1;
            pok = ((pok - float.Parse(scrap)) / pok) * 100;
            return pok < 0 ? 0 : pok > 100 ? 100 : pok;
        }

        public double prep(string ok, string rep)
        {
            float pok = ok != "0" ? float.Parse(ok) : 1;
            pok = ((pok - float.Parse(rep)) / pok) * 100;
            pok = pok < 0 ? 0 : pok > 100 ? 100 : pok;
            return Math.Round(pok, 2);
        }

        public int rep(string[] scr)
        {
            int repair = 0;
            for (int i = 0; i < scr.Length; i++)
            {
                repair += int.Parse(scr[i]);
            }
            return repair;
        }

        public double otr(string effective, string jam_kerja)
        {
            float hsl = (float.Parse(effective) / float.Parse(jam_kerja)) * 100;
            hsl = hsl < 0 ? 0 : hsl > 100 ? 100 : hsl;
            return Math.Round(hsl, 2);
        }

        public double per(string act, string plan)
        {
            float hsl = (float.Parse(act) / float.Parse(plan)) * 100;
            hsl = hsl < 0 ? 0 : hsl > 100 ? 100 : hsl;
            return Math.Round(hsl, 2);
        }

        public double qr(string part_ok, string part_ng)
        {
            int pOk = int.Parse(part_ok);
            float hsl = ((pOk - float.Parse(part_ng)) / (pOk > 0 ? pOk : 1)) * 100;
            hsl = hsl < 0 ? 0 : hsl > 100 ? 100 : hsl;
            return Math.Round(hsl, 2);
        }

        public double oee(double otr, double per, double qr)
        {
            double hsl = ((otr / 100) * (per / 100) * (qr / 100)) * 100;
            hsl = hsl < 0 ? 0 : hsl > 100 ? 100 : hsl;
            return Math.Round(hsl, 2);
        }

        //public double plan(string cycle_time, string jam_kerja)
        //{
        //    float hsl = (60 / float.Parse(cycle_time)) * float.Parse(jam_kerja);
        //    return Math.Round(hsl, 0);
        //}

        public double effective(string jam_kerja, string downtime)
        {
            float hsl = float.Parse(jam_kerja) - float.Parse(downtime);
            return Math.Round(hsl, 0);
        }

        public float dtmin(string downtime)
        {
            float jm = 0;
            string[] dt = downtime.Split(':');
            int par = 0;
            if (int.TryParse(dt[0], out par))
            {
                jm = par == 0 ? 0 : (60 * par);
                par = int.TryParse(dt[1], out par) ? par : 0;
                jm = jm + par;
            }
            return jm;
        }

        public void mHitung(int mode, Model.Machine m, int actc)
        {
            bool added = m.AddAct(actc);
            m.plan = plan(m.work, m.ctime).ToString();
            m.bal = balance(m.act.ToString(), m.plan).ToString();
            m.eff = effective(m.work, dtmin(m.tdown).ToString()).ToString();
            m.otr = per(m.act.ToString(), m.plan);
            m.per = otr(m.eff, m.work);
            m.qr = qr(m.act.ToString(), m.ng);
            m.oee = oee(m.otr, m.per, m.qr);
            if ((mode == 5 ? added : true))
            {
                m.addData(mode);
            }
        }
    }
}
