using System.Collections.Generic;
using System.Linq;

namespace OSPF
{
    public class Traversal
    {

        private const char separator = ':';

        public static Dictionary<string, string> Dijkstra(Graph graph, string source)
        {
            string[] routerIds = graph.Nodes.ToArray();
            int[] distances = Enumerable
                .Repeat(int.MaxValue, routerIds.Length)
                .ToArray();
            for (int i = 0; i < distances.Length; i++)
            {
                if (routerIds[i] == source)
                {
                    distances[i] = 0;
                }
            }

            string[] queue = graph.Nodes.ToArray();

            Dictionary<string, string> pred = new Dictionary<string, string>();
            bool[] visited = new bool[routerIds.Length];



            for (int i = 0; i < distances.Length; i++)
            {
                //susirandi artimiausią dabartiniam routeriui routerį
                //tai iš pradžių next = 0, nes self dist = 0
                int next = MinDistanceIndex(distances, visited);
                if (next == -1)
                    break;
                visited[next] = true;
                //susirenki "next" (kas yra smallest distance node'as) kaimynus
                string[] neighbors = graph.GetNeighbors(routerIds[next]);
                //eini per "next" kaimynus (tai pradedi nuo sellf)
                for (int j = 0; j < neighbors.Length; j++)
                {
                    //pasiimi 'next' kaimyno indexą
                    /*int number = routerIds
                        .FirstIndex(id => id == neighbors[j]);*/
                    int number = 0;
                    for (int h = 0; h < routerIds.Length; h++)
                    {
                        if (routerIds[h] == neighbors[j])
                        {
                            number = h;
                        }
                    }

                    //d - bendra kaina/atstumas
                    //gaunama paėmus atstumą nusigauti iki 
                    //d + cost('next', kažkuris next kaimynas)
                    int d = distances[next] + graph.GetCost(routerIds[next], routerIds[number]);
                    //jeigu čia "pigiausias" būdas nusigauti iki to "next" kaimyno
                    //tai įsimenam tą kainą
                    //ir įsimenam koks pathas
                    if (distances[number] > d)
                    {
                        distances[number] = d;
                        queue[number] = queue[next] + separator + queue[number];
                    }

                }
            }

            foreach (string str in queue)
            {
                string[] parts = str.Split(separator);
                if (parts.Length != 1)
                {
                    pred[parts[parts.Length - 1]] = parts[1];
                }
                else
                {
                    pred[parts[0]] = parts[0];
                }
            }

            return pred;
        }

        private static int MinDistanceIndex(int[] dist, bool[] visited)
        {
            int max = int.MaxValue;
            int index = -1;
            for (int i = 0; i < dist.Length; i++)
            {
                if (!visited[i] && dist[i] < max)
                {
                    index = i;
                    max = dist[i];
                }
            }
            return index;
        }

    }
}