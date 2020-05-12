using System;
// Paxydev
// Frank Kuenneke
// Feel free to fork and do your own stuff with it :)
// Currently not in working order... relatable I guess
// Licensed under GNU GPL 3.0

/*USEFUL FORCES: 
 * 500 - Rare 100%
 * 250 - Magic 100%
 * 100 - Normal 100% (why?)
 */
namespace MagicItemGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetMod.GetItem(500));
            Console.WriteLine(GetMod.GetItem(500)); 
            Console.WriteLine(GetMod.GetItem(500)); 
            Console.WriteLine(GetMod.GetItem(500));
        }
    }
}
