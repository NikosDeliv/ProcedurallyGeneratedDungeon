using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DungeonGenerator.Dungeon;

namespace DungeonGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Instantiate the randomizer and logger
            IRandomize rnd = new Randomize();
            Action<string> logger = Console.WriteLine;

            // Create the Dungeon instance
            Dungeon dungeon = new Dungeon(rnd, logger);

            // Call dungeon generation method
            dungeon.CreateDungeon(100, 100, 20); // dimensions and object count

            // Display the dungeon map
            dungeon.ShowDungeon();
        }
    }
}
