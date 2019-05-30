using System;
using System.Collections.Generic;

namespace OSPF
{
    class Program
    {
        static void Main(string[] args)
        {
            Network network = new Network();

            network.AddRouter("R1");
            network.AddRouter("R2");
            network.AddRouter("R3");
            network.AddRouter("R4");
            network.AddRouter("R5");
            network.AddRouter("R6");
            network.AddRouter("R7");
            network.AddRouter("R8");

            network.AddLink("R1", "R2", 2);
            network.AddLink("R2", "R3", 3);
            network.AddLink("R3", "R7", 9);
            network.AddLink("R3", "R5", 4);
            network.AddLink("R7", "R4", 7);
            network.AddLink("R4", "R6", 8);
            network.AddLink("R5", "R6", 5);
            network.AddLink("R1", "R6", 6);

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
