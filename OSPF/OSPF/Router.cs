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
        private Dictionary<string, string> connections;
        private List<Router> neighbors;
        private NetworkGraph network;
        private List<int> packets;

        public Router(string id)
        {
            Id = id;
            neighbors = new List<Router>();
            packets = new List<int>();
            network = new NetworkGraph(15); //kiek routeriu norim
            network.addEdge(id);
            connections = new Dictionary<string, string>();
            connections.Add(id, id);
        }

        public Dictionary<string, string> GetList()
        {
            return connections;
        }

        private void AddNeighbor(Router router)
        {
            neighbors.Add(router);
        }

        public Router[] GetNeighbors()
        {
            return neighbors.ToArray();
        }

        public NetworkGraph GetNetwork()
        {
            return network;
        }

        public void AddLink(Router router, int weight)
        {
            neighbors.Add(router);
            router.AddNeighbor(this);
            network.addEdge(router.Id);
            network.setLink(Id, router.Id, weight);
            ReceivePacket(GeneratePacket(router));
        }

        public void RemoveLink(Router router)
        {
            neighbors.Remove(router);
            network.removeLink(Id, router.Id);
            ReceivePacket(new Packet(Packet.GetCounter(), Id, network));
        }

        public void RemoveRouter(Router router)
        {
            neighbors.Remove(router);
            network.removeEdge(router.Id);
            ReceivePacket(new Packet(Packet.GetCounter(), Id, network));
        }

        private Packet GeneratePacket(Router router)
        {
            NetworkGraph secondNetwork = router.GetNetwork();

            foreach(string edge in secondNetwork.getEdges())
            {
                network.addEdge(edge);
                foreach(string neighbor in secondNetwork.getNeighbors(edge))
                {
                    network.addEdge(neighbor);
                    network.setLink(edge, neighbor, secondNetwork.getWeight(edge, neighbor));
                }
            }

            return new Packet(Packet.GetCounter(), Id, network);
        }

        private void SendPacket(Packet packet)
        {
            foreach (Router router in neighbors)
            {
                router.ReceivePacket(packet);
            }
        }

        public void ReceivePacket(Packet packet)
        {
            if (!packets.Contains(packet.GetNumber()))
            {
                packets.Add(packet.GetNumber());
                network = packet.GetNetwork();
                connections = Traversal.dijkstra(network, Id);
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
                connections.TryGetValue(dest, out sendTo);
                foreach(Router router in neighbors)
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
