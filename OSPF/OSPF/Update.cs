using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSPF
{
    public class Update
    {

        private static int _counter;
        public static int Counter
        {
            private set => _counter = value;
            get => _counter++;
        }
        public int Number { private set; get; }
        public string Sender { private set; get; }
        public Graph Network { private set; get; }
        public Update(int number, string sender, Graph network)
        {
            Number = number;
            Sender = sender;
            Network = network;
        }
    }
}
