using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OSPF
{
    public class Network
    {

        private List<Router> _routers;

        public Network()
        {
            _routers = new List<Router>();
        }
        /// <summary>
        /// Returns the connections dictionary for a given router Id
        /// </summary>
        /// <param name="routerId"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetConnections(string routerId)
        {
            return _routers
                .FirstOrDefault(r => r.Id == routerId)
                ?.Connections;
        }

        public bool RouterExists(string id)
        {
            return _routers.Any(router => router.Id == id);
        }

        /// <summary>
        /// Checks if a router exists, if not, adds to network
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void AddRouter(string id)
        {
            if (_routers.Any(router => router.Id == id))
                throw new ArgumentException($"router with id: {id} already exists");
            _routers.Add(new Router(id));
        }

        /// <summary>
        /// Finds a router, removes it from the routing tables of all it's neighbors
        /// </summary>
        /// <param name="id"></param>
        /// <returns>If could find a router by given id</returns>
        public void RemoveRouter(string id)
        {
            Router router = _routers
                .FirstOrDefault(r => r.Id == id);
            if (router == null)
                throw new ArgumentException($"router with id: {id} does not exist");
            
            router.Neighbors
                .ForEach(neighbor => neighbor.RemoveRouter(router));

            _routers.Remove(router);
        }

        /// <summary>
        /// Adds a link from source -> destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        public void AddLink(string source, string destination, int cost)
        {
            Router first = _routers
                .FirstOrDefault(r => r.Id == source);
            Router second = _routers
                .FirstOrDefault(r => r.Id == destination);

            if (first == null || second == null)
                throw new ArgumentException($"{source} => {first} {destination} => {second}");

            first.AddLink(second, cost);
        }

        /// <summary>
        /// Removes source -> destination and destination -> source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public void RemoveLink(string source, string destination)
        {
            Router first = _routers
                .FirstOrDefault(r => r.Id == source);
            Router second = _routers
                .FirstOrDefault(r => r.Id == destination);

            if (first == null || second == null)
                throw new ArgumentException($"{source} => {first} {destination} => {second}");

            first.RemoveLink(second);
            second.RemoveLink(first);
        }

        /// <summary>
        /// Sends a message from source to destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="data"></param>
        /// <returns>if could find source</returns>
        public void TransferData(string source, string destination, string data)
        {
            Router router = _routers.FirstOrDefault(r => r.Id == source);
            if (router == null)
                throw new ArgumentException($"Given source router: {source} does not exist");
            try
            {
                /*var thread = new Thread(() => router.SendData(destination, data));
                thread.Start();
                while (thread.ThreadState == ThreadState.Running)
                {
                    Thread.Sleep(50);
                }*/
                router.SendData(destination, data);
            }
            catch
            {
                throw;
            }
        }
    }
}