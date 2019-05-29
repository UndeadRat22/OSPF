using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSPF
{
    public class Router
    {
        public string Id { get; private set; }
        public Dictionary<string, string> Connections { get; private set; }
        public List<Router> Neighbors { get; private set; }
        public NetworkGraph Network { get; private set; }
        private List<int> _packets;

        public Router(string id)
        {
            Id = id;
            Neighbors = new List<Router>();
            _packets = new List<int>();
            Network = new NetworkGraph(15); //kiek routeriu norim
            Network.AddNode(id);
            Connections = new Dictionary<string, string>
            {
                {id, id }
            };
        }

        public void AddLink(Router router, int cost)
        {
            Neighbors.Add(router);
            router.Neighbors.Add(this);

            Network.AddNode(router.Id);
            Network.SetLink(Id, router.Id, cost);

            ReceivePacket(GeneratePacket(router));
        }

        public void RemoveLink(Router router)
        {
            Neighbors.Remove(router);
            Network.RemoveLink(Id, router.Id);
            ReceivePacket(new Packet(Packet.GetCounter(), Id, Network));
        }

        public void RemoveRouter(Router router)
        {
            Neighbors.Remove(router);
            Network.RemoveNode(router.Id);
            ReceivePacket(new Packet(Packet.GetCounter(), Id, Network));
        }

        private Packet GeneratePacket(Router other)
        {
            NetworkGraph secondNetwork = other.Network;

            foreach(string edge in secondNetwork.Nodes)
            {
                Network.AddNode(edge);
                foreach(string neighbor in secondNetwork.GetNeighbors(edge))
                {
                    Network.AddNode(neighbor);
                    Network.SetLink(edge, neighbor, secondNetwork.GetCost(edge, neighbor));
                }
            }

            return new Packet(Packet.GetCounter(), Id, Network);
        }

        private void SendPacket(Packet packet)
        {
            foreach (Router router in Neighbors)
            {
                router.ReceivePacket(packet);
            }
        }

        public void ReceivePacket(Packet packet)
        {
            if (!_packets.Contains(packet.GetNumber()))
            {
                _packets.Add(packet.GetNumber());
                Network = packet.GetNetwork();
                Connections = Traversal.dijkstra(Network, Id);
                SendPacket(packet);
            }
        }

        public void SendMessage(string destination, string data)
        {
            Program.Write("Zinute '" + data + "' routeryje vardu - " + Id);
            Thread.Sleep(200);
            if (destination == Id)
            {
                Program.Write("Zinute '" + data + " pasieke norima routeri vardu - " + Id);
                Program.Write(" ");
            } else
            {
                //sendTo => kam reikia siųsti message'ą kad šis persiųstų arčiau
                //destinationso
                Connections.TryGetValue(destination, out string sendTo);
                if (sendTo == null)
                    throw new ArgumentException($"Can't reach the given destination ({destination}) from {Id}");
                foreach (Router router in Neighbors)
                {
                    if(sendTo != null && sendTo == router.Id)
                    {
                        router.SendMessage(destination, data);
                        break;
                    }
                }
            }
        }
    }
}
