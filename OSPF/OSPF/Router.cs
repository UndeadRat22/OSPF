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
        public Graph Network { get; private set; }
        private List<int> _packets;

        public Router(string id)
        {
            Id = id;
            Neighbors = new List<Router>();
            _packets = new List<int>();
            Network = new Graph();
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

            var update = UpdateNetwork(router);
            ReceiveUpdate(update);
        }

        public void RemoveLink(Router router)
        {
            Neighbors.Remove(router);
            Network.RemoveLink(Id, router.Id);
            ReceiveUpdate(new Update(Update.Counter, Id, Network));
        }

        public void RemoveRouter(Router router)
        {
            Neighbors.Remove(router);
            Network.RemoveNode(router.Id);
            ReceiveUpdate(new Update(Update.Counter, Id, Network));
        }

        private Update UpdateNetwork(Router other)
        {
            Graph otherNetwork = other.Network;

            foreach(string routerId in otherNetwork.Nodes)
            {
                Network.AddNode(routerId);
                foreach(string neighbor in otherNetwork.GetNeighbors(routerId))
                {
                    Network.AddNode(neighbor);
                    Network.SetLink(routerId, neighbor, otherNetwork.GetCost(routerId, neighbor));
                }
            }

            return new Update(Update.Counter, Id, Network);
        }

        private void SendPacket(Update packet)
        {
            Neighbors
                .ForEach(neighbor => neighbor.ReceiveUpdate(packet));
        }

        public void ReceiveUpdate(Update packet)
        {
            if (_packets.Contains(packet.Number))
                return;
            _packets.Add(packet.Number);
            Network = packet.Network;
            Connections = Traversal.Dijkstra(Network, Id);
            SendPacket(packet);
        }

        public bool SendMessage(string destination, string data, string path = "")
        {
            path = $"{path}->{Id}";
            Program.Write($"current path: {path}, data: {data}.");
            if (Id == destination)
            {
                Program.Write("destination reached.");
                return true;
            }
            Connections.TryGetValue(destination, out string sendTo);
            if (sendTo == null)
                return false;
            Program.Write($"Sending {data} to {sendTo}, final destination: {destination}");
            Thread.Sleep(Settings.NetworkDelayTime);

            Router next = Neighbors.FirstOrDefault(router => router.Id == sendTo);
            return next.SendMessage(destination, data, path);
        }
    }
}
