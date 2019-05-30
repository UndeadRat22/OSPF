using System.Collections.Generic;
using System.Linq;

using OSPF.Settings;

namespace OSPF
{
    public class LinkStateDatabase
    {

        private int[][] _networkNodes;
        private IDictionary<string, int> _map;
        private List<int> _emptyMatrixSlots;

        public IEnumerable<string> Nodes { get { return _map.Keys; } }

        public LinkStateDatabase()
        {
            _networkNodes = Utility.Matrix(NetworkSettings.MaxNetworkSize);
            _map = new Dictionary<string, int>();
            _emptyMatrixSlots = Enumerable
                .Range(0, NetworkSettings.MaxNetworkSize)
                .ToList();
        }

        public bool AddNode(string routerId)
        {
            if (_map.ContainsKey(routerId) || _emptyMatrixSlots.IsEmpty())
                return false;

            int matrixIndex = _emptyMatrixSlots.First();
            _emptyMatrixSlots.RemoveAt(0);
            _map[routerId] = matrixIndex;

            return true;
        }

        public bool RemoveNode(string router)
        {
            if (!_map.ContainsKey(router))
                return false;

            _map.TryGetValue(router, out int index);
            _map.Remove(router);
            _emptyMatrixSlots.Add(index);
            for (int i = 0; i < _networkNodes[index].Length; i++)
            {
                _networkNodes[index][i] = 0;
                _networkNodes[i][index] = 0;
            }
            return true;
        }

        public bool SetLink(string first, string second, int cost)
        {
            if (!_map.ContainsKey(first) || !_map.ContainsKey(second))
                return false;

            _networkNodes[_map[first]][_map[second]] = cost;
            _networkNodes[_map[second]][_map[first]] = cost;

            return true;
        }

        public bool RemoveLink(string first, string second)
        {
            if (!_map.ContainsKey(first) || !_map.ContainsKey(second))
                return false;
            _networkNodes[_map[first]][_map[second]] = 0;
            _networkNodes[_map[second]][_map[first]] = 0;
            return true;
        }

        public int GetCost(string first, string second)
        {
            if (!_map.ContainsKey(first) || !_map.ContainsKey(second))
                return -1;

            return _networkNodes[_map[first]][_map[second]];
        }

        public string[] GetNeighbors(string routerId)
        {
            if (!_map.ContainsKey(routerId))
                return null;
            _map.TryGetValue(routerId, out int matrixIndex);

            var numbers = _networkNodes[matrixIndex]
                .IndicesWhere(edge => edge > 0);

            string[] neigbors = _map.Keys
                .Where(key => numbers.Contains(_map[key]))
                .ToArray();
            return neigbors;

        }
    }
}