using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndonModbus.Model
{
    class Buttons
    {
        public string id = "";
        public string code = "";
        public int type;

        public Buttons() { }
        public Buttons(string id, string code, int type)
        {
            this.id = id;
            this.code = code;
            this.type = type;
        }
    }
}
