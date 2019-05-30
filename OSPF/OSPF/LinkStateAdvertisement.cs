using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSPF
{
    public class LinkStateAdvertisement
    {

        private static int _counter;
        public static int Counter
        {
            private set => _counter = value;
            get => _counter++;
        }
        public int Number { private set; get; }
        public string Sender { private set; get; }
        public LinkStateDatabase Network { private set; get; }
        public LinkStateAdvertisement(int number, string sender, LinkStateDatabase network)
        {
            Number = number;
            Sender = sender;
            Network = network;
        }
    }
}
