using System.Collections.Generic;
using System.Linq;

namespace OSPF
{
    public class NetworkGraph
    {

        private int[][] edges;
        private IDictionary<string, int> map;
        private LinkedList<int> freeCells;
        private int freeIndex;

        public NetworkGraph(int maxRouters)
        {
            freeIndex = 0;
            edges = RectangularArrays.ReturnRectangularIntArray(maxRouters, maxRouters);
            map = new Dictionary<string, int>();
            freeCells = new LinkedList<int>();

        }

        public virtual int Size
        {
            get
            {
                return map.Count;
            }
        }

        public IEnumerable<string> GetEdges()
        {
            return map.Keys;
        }

        public bool AddEdge(string router)
        {
            int index;
            if (map.ContainsKey(router))
            {
                return false;
            }
            if (freeCells.Count == 0)
            {
                index = freeIndex;
                map[router] = index;
                freeIndex++;
            }
            else
            {
                index = freeCells.First.Value;
                freeCells.RemoveFirst();
                map[router] = index;
            }
            return true;
        }

        public bool RemoveEdge(string router)
        {
            int index;
            if (map.ContainsKey(router))
            {
                map.TryGetValue(router, out index);
                map.Remove(router);
                freeCells.AddLast(index);
                for (int i = 0; i < edges[index].Length; i++)
                {
                    edges[index][i] = 0;
                    edges[i][index] = 0;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetLink(string source, string dest, int cost)
        {
            if (map.ContainsKey(source) && map.ContainsKey(dest))
            {
                edges[map[source]][map[dest]] = cost;
                edges[map[dest]][map[source]] = cost;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveLink(string source, string dest)
        {
            if (map.ContainsKey(source) && map.ContainsKey(dest))
            {
                edges[map[source]][map[dest]] = 0;
                edges[map[dest]][map[source]] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetCost(string source, string dest)
        {
            if (map.ContainsKey(source) && map.ContainsKey(dest))
            {
                return edges[map[source]][map[dest]];
            }
            else
            {
                return -1;
            }
        }

        public string[] GetNeighbors(string router)
        {
            int[] numbers;
            string[] neighbors;
            if (map.ContainsKey(router))
            {
                int count = 0;
                int index;
                map.TryGetValue(router, out index);
                for (int i = 0; i < edges.Length; i++)
                {
                    if (edges[index][i] > 0)
                    {
                        count++;
                    }
                }
                numbers = new int[count];
                neighbors = new string[count];
                count = 0;
                for (int i = 0; i < edges.Length; i++)
                {
                    if (edges[index][i] > 0)
                    {
                        numbers[count++] = i;
                    }
                }
                count = 0;
                foreach (string id in map.Keys)
                {
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        if (numbers[i] == map[id])
                        {
                            neighbors[count++] = id;
                            break;
                        }
                    }
                }
                return neighbors;
            }
            else
            {
                return null;
            }
        }
    }
}