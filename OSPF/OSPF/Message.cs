using System.Threading;

namespace OSPF
{
    public struct Message
    {

        public Router Router { get; set; }
        public string Destination { get; set; }
        public string Data { get; set; }

        public void Send()
        {
            Router.SendMessage(Destination, Data);
        }
    }
}
