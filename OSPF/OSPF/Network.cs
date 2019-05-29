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

        /// <summary>
        /// Checks if a router exists, if not, adds to network
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AddRouter(string id)
        {
            if (_routers.Any(router => router.Id == id))
                return false;
            _routers.Add(new Router(id));
            return true;
        }

        /// <summary>
        /// Finds a router, removes it from the routing tables of all it's neighbors
        /// </summary>
        /// <param name="id"></param>
        /// <returns>If could find a router by given id</returns>
        public bool RemoveRouter(string id)
        {
            Router router = _routers
                .FirstOrDefault(r => r.Id == id);
            if (router == null)
                return false;
            
            router.Neighbors
                .ForEach(neighbor => neighbor.RemoveRouter(router));

            _routers.Remove(router);

            return true;
        }

        /// <summary>
        /// Adds a link from source -> destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        public bool AddLink(string source, string destination, int cost)
        {
            Router first = _routers
                .FirstOrDefault(r => r.Id == source);
            Router second = _routers
                .FirstOrDefault(r => r.Id == destination);

            if (first == null || second == null)
                return false;

            first.AddLink(second, cost);
            return true;
        }

        /// <summary>
        /// Removes source -> destination and destination -> source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public bool RemoveLink(string source, string destination)
        {
            Router first = _routers
                .FirstOrDefault(r => r.Id == source);
            Router second = _routers
                .FirstOrDefault(r => r.Id == destination);

            if (first == null || second == null)
                return false;

            first.RemoveLink(second);
            second.RemoveLink(first);

            return true;
        }

        /// <summary>
        /// Sends a message from source to destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        /// <returns>if could find source</returns>
        public bool SendMessage(string source, string destination, string message)
        {
            Router router = _routers.FirstOrDefault(r => r.Id == source);
            if (router == null)
                return false;
            Message msg = new Message
            {
                Router = router,
                Destination = destination,
                Data = message
            };
            Thread thread = new Thread(msg.Send);
            thread.Start();
            return true;
        }
    }
}