using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OSPF
{
    public class Network
    {

        private List<Router> routers;

        public Network()
        {
            routers = new List<Router>();
        }

        public Dictionary<string, string> GetConnections(string source)
        {
            return routers
                .FirstOrDefault(r => r.Id == source)
                ?.GetList();
        }

        /// <summary>
        /// Checks if a router exists, if not, adds to network
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AddRouter(string id)
        {
            if (routers.Any(router => router.Id == id))
                return false;
            routers.Add(new Router(id));
            return true;
        }

        public bool RemoveRouter(string id)
        {
            Router router = routers
                .FirstOrDefault(r => r.Id == id);
            if (router == null)
                return false;
            
            router.GetNeighbors()
                .ForEach(neighbor => neighbor.RemoveRouter(router));

            routers.Remove(router);

            return true;
        }

        public virtual bool AddLink(string source, string dest, int weight)
        {
            foreach (Router router1 in routers)
            {
                if (source.Equals(router1.Id))
                {
                    foreach (Router router2 in routers)
                    {
                        if (dest.Equals(router2.Id))
                        {
                            router1.AddLink(router2, weight);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public virtual bool RemoveLink(string source, string dest)
        {
            foreach (Router router1 in routers)
            {
                if (source.Equals(router1.Id))
                {
                    foreach (Router router2 in routers)
                    {
                        if (dest.Equals(router2.Id))
                        {
                            router1.RemoveLink(router2);
                            router2.RemoveLink(router1);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public virtual bool SendMessage(string source, string dest, string message)
        {
            foreach (Router router in routers)
            {
                //jeigu source == routerid -> reiškia iš šito routerio reikia išsiųst
                if (source.Equals(router.Id))
                {
                    Message msg = new Message
                    {
                        Router = router,
                        Destination = dest,
                        Data = message
                    };
                    Thread thread = new Thread(msg.Send);
                    thread.Start();
                    return true;
                }
            }
            return false;
        }
    }
}