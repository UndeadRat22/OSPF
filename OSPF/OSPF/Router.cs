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
        private List<int> packets;

        public Router(string id)
        {
            Id = id;
            Neighbors = new List<Router>();
            packets = new List<int>();
            Network = new NetworkGraph(15); //kiek routeriu norim
            Network.addEdge(id);
            Connections = new Dictionary<string, string>
            {
                {id, id }
            };
        }

        private void AddNeighbor(Router router)
        {
            Neighbors.Add(router);
        }

        public void AddLink(Router router, int weight)
        {
            Neighbors.Add(router);
            router.AddNeighbor(this);
            Network.addEdge(router.Id);
            Network.setLink(Id, router.Id, weight);
            ReceivePacket(GeneratePacket(router));
        }

        public void RemoveLink(Router router)
        {
            Neighbors.Remove(router);
            Network.removeLink(Id, router.Id);
            ReceivePacket(new Packet(Packet.GetCounter(), Id, Network));
        }

        public void RemoveRouter(Router router)
        {
            Neighbors.Remove(router);
            Network.removeEdge(router.Id);
            ReceivePacket(new Packet(Packet.GetCounter(), Id, Network));
        }

        private Packet GeneratePacket(Router router)
        {
            NetworkGraph secondNetwork = router.Network;

            foreach(string edge in secondNetwork.getEdges())
            {
                Network.addEdge(edge);
                foreach(string neighbor in secondNetwork.getNeighbors(edge))
                {
                    Network.addEdge(neighbor);
                    Network.setLink(edge, neighbor, secondNetwork.getWeight(edge, neighbor));
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
            if (!packets.Contains(packet.GetNumber()))
            {
                packets.Add(packet.GetNumber());
                Network = packet.GetNetwork();
                Connections = Traversal.dijkstra(Network, Id);
                SendPacket(packet);
            }
        }

        public void SendMessage(string dest, string text)
        {
            Program.Write("Zinute '" + text + "' routeryje vardu - " + Id);
            Thread.Sleep(5000);
            if (dest == Id)
            {
                Program.Write("Zinute '" + text + " pasieke norima routeri vardu - " + Id);
                Program.Write(" ");
            } else
            {
                string sendTo;
                Connections.TryGetValue(dest, out sendTo);
                foreach(Router router in Neighbors)
                {
                    if(sendTo != null && sendTo == router.Id)
                    {
                        router.SendMessage(dest, text);
                        break;
                    }
                }
            }
        }
    }
}
