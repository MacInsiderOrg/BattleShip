using System;
using System.ServiceModel;

namespace GameHost
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "BattleShip Server";
            Console.ForegroundColor =  ConsoleColor.White;            
            
            var host = new ServiceHost(typeof(GameService.BattleShipService));
            string answer;

            do
            {
                Console.WriteLine("Select action:\n  -run (r)\n  -stop (s)\n  -exit (e)");
                answer = Console.ReadLine();

                switch (answer)
                {
                    case "r":
                        host = new ServiceHost(typeof(GameService.BattleShipService));
                        host.Open();
                        Console.WriteLine("\n---Host started...\n");
                        break;

                    case "s":
                        host.Close();
                        Console.WriteLine("\n---Host closed...\n");
                        break;
                }
            } while (answer != "e");
        }
    }
}
