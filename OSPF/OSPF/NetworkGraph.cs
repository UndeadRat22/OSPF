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

        public NetworkGraph(int routerCount)
        {
            freeIndex = 0;
            edges = Utility.Matrix(routerCount);
            map = new Dictionary<string, int>();
            freeCells = new LinkedList<int>();

        }

        public int Size
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

        public bool AddEdge(string routerId)
        {
            if (map.ContainsKey(routerId))
                return false;

            int index;
            if (freeCells.Count == 0)
            {
                index = freeIndex;
                map[routerId] = index;
                freeIndex++;
            }
            else
            {
                index = freeCells.First.Value;
                freeCells.RemoveFirst();
                map[routerId] = index;
            }
            return true;
        }

        public bool RemoveEdge(string router)
        {
            int index;
            if (!map.ContainsKey(router))
                return false;
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

        public bool SetLink(string first, string second, int cost)
        {
            if (!map.ContainsKey(first) || !map.ContainsKey(second))
                return false;

            edges[map[first]][map[second]] = cost;
            edges[map[second]][map[first]] = cost;

            return true;
        }

        public bool RemoveLink(string first, string second)
        {
            if (!map.ContainsKey(first) || !map.ContainsKey(second))
                return false;
            edges[map[first]][map[second]] = 0;
            edges[map[second]][map[first]] = 0;
            return true;
        }

        public int GetCost(string first, string second)
        {
            if (!map.ContainsKey(first) || !map.ContainsKey(second))
                return -1;

            return edges[map[first]][map[second]];
        }

        public string[] GetNeighbors(string routerId)
        {
            if (!map.ContainsKey(routerId))
                return null;

            int matrixIndex;
            map.TryGetValue(routerId, out matrixIndex);

            var numbers = edges[matrixIndex]
                .IndicesWhere(edge => edge > 0);

            string[] neigbors = map.Keys
                .Where(key => numbers.Contains(map[key]))
                .ToArray();
            return neigbors;

        }
    }
}