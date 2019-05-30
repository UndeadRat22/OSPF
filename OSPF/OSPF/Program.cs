using OSPF.Settings;
using System;
using System.Collections.Generic;

namespace OSPF
{
    class Program
    {
        //Open Shortest Path Fist
        static void Main(string[] args)
        {
            RouterSettings.LoadSettings("router_settings.json");
            var settings = RouterSettings.Settings;

            Network network = new Network();
            network.UseSettings(settings);

            Menu menu = new Menu(network);

            while (true)
            {
                menu.ShowOptions();
                try
                {
                    menu.SelectOption();
                } catch (Exception e)
                {
                    Console.WriteLine($"Action failed, reason {e.Message}");
                }
            }
        }
    }
}
