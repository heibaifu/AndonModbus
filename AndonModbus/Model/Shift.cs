using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndonModbus.Model
{
    class Shift
    {
        public bool next = false, allow = true;
        public string id = "", name = "", start = "00:00:00", stop = "00:00:00", br = "0", diff = "0";

        public Shift(string id, string name, string start, string stop, string br, bool next = false, bool allow = true)
        {
            this.id = id;
            this.name = name;
            this.start = start;
            this.stop = stop;
            this.br = br;
            this.next = next;
            this.allow = allow;
        }
    }
}
