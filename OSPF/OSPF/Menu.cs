using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSPF
{
    public class Menu
    {
        private Network _network;

        private List<KeyValuePair<string, Action>> _options;
        private string _selectedRouter = null;

        private bool Selected { get => _selectedRouter != null; }
        public Menu(Network network)
        {
            _network = network;
            _options = new List<KeyValuePair<string, Action>>()
            {
                new KeyValuePair<string, Action>("Add Router", AddRouter),
                new KeyValuePair<string, Action>("Remove Router", RemoveRouter),
                new KeyValuePair<string, Action>("Select Router", SelectMainRouter),
                new KeyValuePair<string, Action>("Add Link", AddLink),
                new KeyValuePair<string, Action>("Show Routes", ShowRoutes),
                new KeyValuePair<string, Action>("Transfer Data", TransferData),
                new KeyValuePair<string, Action>("Quit", Quit),
            };
        }

        public void ShowOptions()
        {
            for (int id = 0; id < _options.Count; id++)
            {
                Console.WriteLine($"{id}: {_options[id].Key}");
            }
        }

        public void SelectOption()
        {
            int.TryParse(Console.ReadLine(), out int index);
            _options[index].Value();
        }

        private void Success() => Console.WriteLine("Successful");

        public void AddRouter()
        {
            Console.WriteLine("Id: ");
            string router = Console.ReadLine();
            try
            {
                _network.AddRouter(router);
            }
            catch
            {
                throw;
            }
            Success();
        }

        public string SelectRouter()
        {
            Console.WriteLine("Id of router:");
            string selection = Console.ReadLine();
            if (!_network.RouterExists(selection))
            {
                return null;
            }
            return selection;
        }
        public void SelectMainRouter()
        {
            _selectedRouter = SelectRouter();
            if (_selectedRouter == null)
            {
                Console.WriteLine("Router doesn't exist");
                return;
            }
            Success();
        }

        public void RemoveRouter()
        {
            if (!Selected)
            {
                Console.WriteLine("Please select a router first!");
                return;
            }
            Console.WriteLine($"Removing {_selectedRouter}");
            _network.RemoveRouter(_selectedRouter);
            _selectedRouter = null;
            Success();
        }

        public void AddLink()
        {
            if (!Selected)
            {
                Console.WriteLine("Please select a router first!");
                return;
            }
            string second = SelectRouter();
            if (second == null)
            {
                Console.WriteLine($"Router {second} doesn't exist");
                return;
            }
            Console.WriteLine($"Cost between {_selectedRouter} and {second}: ");
            int cost = int.Parse(Console.ReadLine());
            _network.AddLink(_selectedRouter, second, cost);
            Success();
        }

        public void ShowRoutes()
        {
            if (!Selected)
            {
                Console.WriteLine("Please select a router first!");
                return;
            }
            _network.GetConnections(_selectedRouter)
                .ForEach(pair => Console.WriteLine($"{pair.Key} => {pair.Value}"));
        }


        public void TransferData()
        {
            if (!Selected)
            {
                Console.WriteLine("Please select a router first!");
                return;
            }
            string destination = SelectRouter();
            if (destination == null)
            {
                Console.WriteLine("destination does not exist");
                return;
            }
            Console.WriteLine("Data: ");
            string data = Console.ReadLine();
            _network.TransferData(_selectedRouter, destination, data);
        }

        public void Quit()
        {
            Environment.Exit(1);
        }
    }
}
