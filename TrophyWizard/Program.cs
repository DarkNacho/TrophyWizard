using System;
using TrophyParser;
using TrophyParser.PS3;
using TrophyParser.Vita;

namespace TrophyWizard
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = args[1];
            IUnlocker unlocker = new PS3Unlocker(dir);
            if (args[0].Equals("PS3")) unlocker = new PS3Unlocker(args[0]);
            else if (args[0].Equals("Vita")) unlocker = new VitaUnlocker(args[0]);
            else throw new Exception("Bad Console");
            

            Console.WriteLine(unlocker.ToString());
            Console.WriteLine("Select Trophy: ");
            int cmd =  Convert.ToInt32(Console.ReadLine());
            while (cmd >= 0 && cmd < unlocker.Count)
            {
            
                var trophy = unlocker[cmd];
                if(!trophy.TrophyInfo.Time.HasValue)
                {
                    Console.WriteLine("Do you want to unlock it  (Y / N)?");
                    var yn = Console.ReadLine();
                    if(yn[0] == 'Y')
                    {
                        Console.WriteLine("When was unlock it? ");
                        unlocker.UnlockTrophy(trophy.Id, DateTime.Parse(Console.ReadLine()));
                    }
                }
                else
                {
                    Console.WriteLine("Do you want to lock it (Y / N )");
                    var yn = Console.ReadLine();
                    if (yn[0] == 'Y') unlocker.LockTrophy(trophy.Id);
                    else
                    {
                        Console.WriteLine("New time:");
                        unlocker.ChangeTime(trophy.Id, DateTime.Parse(Console.ReadLine()));
                    }
                }

                Console.WriteLine(unlocker.ToString());
                cmd = Convert.ToInt32(Console.ReadLine());
            }
            unlocker.Save();
        }
    }
}
