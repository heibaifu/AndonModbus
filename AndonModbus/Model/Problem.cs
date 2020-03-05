namespace AndonModbus.Model
{
    class Problem
    {
        public string id = "", number = "", name = "", allow = "";

        public Problem(string id, string number, string name, string allow)
        {
            this.id = id;
            this.number = number;
            this.name = name;
            this.allow = allow;
        }
    }
}
