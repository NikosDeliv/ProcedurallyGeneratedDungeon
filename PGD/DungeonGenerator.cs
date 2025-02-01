using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Numerics;


namespace DungeonGenerator
{
   
    public class Dungeon
      {

        const string MsgXSize = "X size of dungeon: \t";

        const string MsgYSize = "Y size of dungeon: \t";

        const string MsgMaxObjects = "max # of objects: \t";

        const string MsgNumObjects = "# of objects made: \t";

        // max size of the map
        int xmax = 80; //columns
        int ymax = 30; //rows

        // size of the map
        int _xsize;
        int _ysize;

        // number of "objects" to generate on the map
        int _objects;

        // define the chance to generate either a room or a corridor on the map, room is high prio so I only do that
       
        const int ChanceRoom = 75;

        // the map
        Tile[] _dungeonMap = { };

        readonly IRandomize _rnd;

        readonly Action<string> _logger;
        public enum Tile // the tiles 
        {
            Unused,
            DirtWall,
            DirtFloor,
            StoneWall,
            Upstairs,
            Downstairs,
            Chest,
            Door,
            Corridor
        }
        public enum Direction // the directions
        {
            North,
            South,
            West,
            East
        }
        public struct PointI
        {
            public int X { get; set; }
            public int Y { get; set; }

            public PointI(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        public interface IRandomize
        {
            int GetRand(int min, int max);
        }

        public class Randomize : IRandomize
        {
            private readonly Random _rnd;

            public Randomize()
            {
                _rnd = new Random();
            }

            public int GetRand(int min, int max)
            {
                return _rnd.Next(min, max); // Use Random's Next method
            }
        }



        public Dungeon(IRandomize rnd, Action<string> logger)
        {
            _rnd = rnd;
            _logger = logger;
        }

        public int Corridors
        {
            get;
            private set;
        }

        public static bool IsWall(int x, int y, int xlen, int ylen, int xt, int yt, Direction d)
        {
            Func<int, int, int> a = GetFeatureLowerBound;

            Func<int, int, int> b = IsFeatureWallBound;
            switch (d)
            {
                case Direction.North:
                    return xt == a(x, xlen) || xt == b(x, xlen) || yt == y || yt == y - ylen + 1;
                case Direction.East:
                    return xt == x || xt == x + xlen - 1 || yt == a(y, ylen) || yt == b(y, ylen);
                case Direction.South:
                    return xt == a(x, xlen) || xt == b(x, xlen) || yt == y || yt == y + ylen - 1;
                case Direction.West:
                    return xt == x || xt == x - xlen + 1 || yt == a(y, ylen) || yt == b(y, ylen);
            }

            throw new InvalidOperationException();
        }

        public static int GetFeatureLowerBound(int c, int len)
        {
            return c - len / 2;
        }

        public static int IsFeatureWallBound(int c, int len)
        {
            return c + (len - 1) / 2;
        }

        public static int GetFeatureUpperBound(int c, int len)
        {
            return c + (len + 1) / 2;
        }

        public static IEnumerable<PointI> GetRoomPoints(int x, int y, int xlen, int ylen, Direction d)
        {
            // north and south share the same x thing
            // east and west share the same y thing
            Func<int, int, int> a = GetFeatureLowerBound;
            Func<int, int, int> b = GetFeatureUpperBound;

            switch (d)
            {
                case Direction.North:
                    for (var xt = a(x, xlen); xt < b(x, xlen); xt++) for (var yt = y; yt > y - ylen; yt--) yield return new PointI { X = xt, Y = yt };
                    break;
                case Direction.East:
                    for (var xt = x; xt < x + xlen; xt++) for (var yt = a(y, ylen); yt < b(y, ylen); yt++) yield return new PointI { X = xt, Y = yt };
                    break;
                case Direction.South:
                    for (var xt = a(x, xlen); xt < b(x, xlen); xt++) for (var yt = y; yt < y + ylen; yt++) yield return new PointI { X = xt, Y = yt };
                    break;
                case Direction.West:
                    for (var xt = x; xt > x - xlen; xt--) for (var yt = a(y, ylen); yt < b(y, ylen); yt++) yield return new PointI { X = xt, Y = yt };
                    break;
                default:
                    yield break;
            }
        }

        public Tile GetCellType(int x, int y)
        {
            if (x < 0 || x >= this._xsize || y < 0 || y >= this._ysize)
            {
                return Tile.Unused; // or any other default tile type
            }
            return this._dungeonMap[x + this._xsize * y];
        }


        public int GetRand(int min, int max)
        {
            return _rnd.GetRand(min, max);
        }

        public bool MakeCorridor(int x, int y, int length, Direction direction)
        {
            int len = this.GetRand(2, length);
            const Tile Floor = Tile.Corridor;
            int xtemp;
            int ytemp = 0;

            switch (direction)
            {
                case Direction.North:
                    if (!InBounds(x, y)) return false;
                    xtemp = x;
                    for (ytemp = y; ytemp > (y - len); ytemp--)
                    {
                        if (!InBounds(xtemp, ytemp) || GetCellType(xtemp, ytemp) != Tile.Unused) return false;
                    }
                    Corridors++;
                    for (ytemp = y; ytemp > (y - len); ytemp--)
                    {
                        this.SetCell(xtemp, ytemp, Floor);
                    }
                    break;

                case Direction.East:
                    if (!InBounds(x, y)) return false;
                    ytemp = y;
                    for (xtemp = x; xtemp < (x + len); xtemp++)
                    {
                        if (!InBounds(xtemp, ytemp) || GetCellType(xtemp, ytemp) != Tile.Unused) return false;
                    }
                    Corridors++;
                    for (xtemp = x; xtemp < (x + len); xtemp++)
                    {
                        this.SetCell(xtemp, ytemp, Floor);
                    }
                    break;

                case Direction.South:
                    if (!InBounds(x, y)) return false;
                    xtemp = x;
                    for (ytemp = y; ytemp < (y + len); ytemp++)
                    {
                        if (!InBounds(xtemp, ytemp) || GetCellType(xtemp, ytemp) != Tile.Unused) return false;
                    }
                    Corridors++;
                    for (ytemp = y; ytemp < (y + len); ytemp++)
                    {
                        this.SetCell(xtemp, ytemp, Floor);
                    }
                    break;

                case Direction.West:
                    if (!InBounds(x, y)) return false;
                    ytemp = y;
                    for (xtemp = x; xtemp > (x - len); xtemp--)
                    {
                        if (!InBounds(xtemp, ytemp) || GetCellType(xtemp, ytemp) != Tile.Unused) return false;
                    }
                    Corridors++;
                    for (xtemp = x; xtemp > (x - len); xtemp--)
                    {
                        this.SetCell(xtemp, ytemp, Floor);
                    }
                    break;
            }
            return true;
        }


        public IEnumerable<Tuple<PointI, Direction>> GetSurroundingPoints(PointI v)
        {
            var points = new[]
                             {
                                 Tuple.Create(new PointI { X = v.X, Y = v.Y + 1 }, Direction.North),
                                 Tuple.Create(new PointI { X = v.X - 1, Y = v.Y }, Direction.East),
                                 Tuple.Create(new PointI { X = v.X , Y = v.Y-1 }, Direction.South),
                                 Tuple.Create(new PointI { X = v.X +1, Y = v.Y  }, Direction.West),

                             };
            return points.Where(p => InBounds(p.Item1));
        }

        public IEnumerable<Tuple<PointI, Direction, Tile>> GetSurroundings(PointI v)
        {
            return
                this.GetSurroundingPoints(v)
                    .Select(r => Tuple.Create(r.Item1, r.Item2, this.GetCellType(r.Item1.X, r.Item1.Y)));
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < this.xmax && y >= 0 && y < this.ymax;
        }


        public bool InBounds(PointI v)
        {
            return this.InBounds(v.X, v.Y);
        }

        public bool MakeRoom(int x, int y, int xlength, int ylength, Direction direction)
        {
            // define the dimensions of the room
            int xlen = this.GetRand(6, xlength);
            int ylen = this.GetRand(6, ylength);

            // the tile type it's going to be filled with
            const Tile Floor = Tile.DirtFloor;

            const Tile Wall = Tile.DirtWall;
            // choose the way it's pointing at

            var points = GetRoomPoints(x, y, xlen, ylen, direction).ToArray();

            // Check if there's enough space left for it
            if (
                points.Any(
                    s =>
                    s.Y < 0 || s.Y > this._ysize || s.X < 0 || s.X > this._xsize || this.GetCellType(s.X, s.Y) != Tile.Unused)) return false;
            _logger(
                      string.Format(
                          "Making room:int x={0}, int y={1}, int xlength={2}, int ylength={3}, int direction={4}",
                          x,
                          y,
                          xlength,
                          ylength,
                          direction));

            foreach (var p in points)
            {
                this.SetCell(p.X, p.Y, IsWall(x, y, xlen, ylen, p.X, p.Y, direction) ? Wall : Floor);
            }

            // we are so goated
            return true;
        }

        public Tile[] GetDungeon()
        {
            return this._dungeonMap;
        }

        public char GetCellTile(int x, int y)
        {
            switch (GetCellType(x, y))
            {
                case Tile.Unused:
                    return ' ';
                case Tile.DirtWall:
                    return '#';
                case Tile.DirtFloor:
                    return '.';
                case Tile.StoneWall:
                    return 'S';
                case Tile.Corridor:
                    return '=';
                case Tile.Door:
                    return 'D';
                case Tile.Upstairs:
                    return '+';
                case Tile.Downstairs:
                    return '-';
                case Tile.Chest:
                    return 'C';
                default:
                    throw new ArgumentOutOfRangeException("x,y");
            }
        }

        //used to print the map on the screen
        public void ShowDungeon()
        {
            for (int y = 0; y < this._ysize; y++)
            {
                for (int x = 0; x < this._xsize; x++)
                {
                    Console.Write(GetCellTile(x, y));
                }

                if (this._xsize <= xmax) Console.WriteLine();
            }
        }

        public Direction RandomDirection()
        {
            int dir = this.GetRand(0, 4);
            switch (dir)
            {
                case 0:
                    return Direction.North;
                case 1:
                    return Direction.East;
                case 2:
                    return Direction.South;
                case 3:
                    return Direction.West;
                default:
                    throw new InvalidOperationException();
            }
        }

        //and here's the one generating the whole map
        public bool CreateDungeon(int inx, int iny, int inobj)
        {
            this._objects = inobj < 1 ? 10 : inobj;

            // adjust the size of the map, if it's smaller or bigger than the limits
            if (inx < 3) this._xsize = 3;
            else if (inx > xmax) this._xsize = xmax;
            else this._xsize = inx;

            if (iny < 3) this._ysize = 3;
            else if (iny > ymax) this._ysize = ymax;
            else this._ysize = iny;

            Console.WriteLine(MsgXSize + this._xsize);
            Console.WriteLine(MsgYSize + this._ysize);
            Console.WriteLine(MsgMaxObjects + this._objects);

            // redefine the map var, so it's adjusted to our new map size
            this._dungeonMap = new Tile[this._xsize * this._ysize];

            // start with making the "standard stuff" on the map
            this.Initialize();

            // start with making a room in the middle, which we can start building upon
            this.MakeRoom(this._xsize / 2, this._ysize / 2, 8, 6, RandomDirection()); 

            // keep count of the number of "objects" we've made
            int currentFeatures = 1; // +1 for the first room we just made

            // then we sart the main loop
            for (int countingTries = 0; countingTries < 1000; countingTries++)
            {
                // check if we've reached our quota
                if (currentFeatures == this._objects)
                {
                    break;
                }

                // start with a random wall
                int newx = 0;
                int xmod = 0;
                int newy = 0;
                int ymod = 0;
                Direction? validTile = null;

                // 1000 chances to find a suitable object (room or corridor)
                for (int testing = 0; testing < 1000; testing++)
                {
                    newx = this.GetRand(1, this._xsize - 1);
                    newy = this.GetRand(1, this._ysize - 1);

                    if (GetCellType(newx, newy) == Tile.DirtWall || GetCellType(newx, newy) == Tile.Corridor)
                    {
                        var surroundings = this.GetSurroundings(new PointI() { X = newx, Y = newy });

                        // check if we can reach the place
                        var canReach =
                            surroundings.FirstOrDefault(s => s.Item3 == Tile.Corridor || s.Item3 == Tile.DirtFloor);
                        if (canReach == null)
                        {
                            continue;
                        }
                        validTile = canReach.Item2;
                        switch (canReach.Item2)
                        {
                            case Direction.North:
                                xmod = 0;
                                ymod = -1;
                                break;
                            case Direction.East:
                                xmod = 1;
                                ymod = 0;
                                break;
                            case Direction.South:
                                xmod = 0;
                                ymod = 1;
                                break;
                            case Direction.West:
                                xmod = -1;
                                ymod = 0;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }


                        // check that we haven't got another door nearby, so we won't get alot of openings besides
                        // each other

                        if (GetCellType(newx, newy + 1) == Tile.Door) // north
                        {
                            validTile = null;

                        }

                        else if (GetCellType(newx - 1, newy) == Tile.Door) // east
                            validTile = null;
                        else if (GetCellType(newx, newy - 1) == Tile.Door) // south
                            validTile = null;
                        else if (GetCellType(newx + 1, newy) == Tile.Door) // west
                            validTile = null;


                        // if we can, jump out of the loop and continue with the rest
                        if (validTile.HasValue) break;
                    }
                }

                if (validTile.HasValue)
                {
                    // choose what to build now at our newly found place, and at what direction
                    int feature = this.GetRand(0, 100);
                    if (feature <= ChanceRoom)
                    { // a new room
                        if (this.MakeRoom(newx + xmod, newy + ymod, 8, 6, validTile.Value))
                        {
                            currentFeatures++; // add to our quota

                            // then we mark the wall opening with a door
                            this.SetCell(newx, newy, Tile.Door);

                            // clean up infront of the door so we can reach it
                            this.SetCell(newx + xmod, newy + ymod, Tile.DirtFloor);
                        }
                    }
                    else if (feature >= ChanceRoom)
                    { // new corridor
                        if (this.MakeCorridor(newx + xmod, newy + ymod, 6, validTile.Value))
                        {
                            // same thing here, add to the quota and a door
                            currentFeatures++;

                            this.SetCell(newx, newy, Tile.Door);
                        }
                    }
                }
            }

            
            AddSprinkles();

            // all done with the map generation, tell the user about it and finish
            Console.WriteLine(MsgNumObjects + currentFeatures);

            return true;
        }

        void Initialize()
        {
            for (int y = 0; y < this._ysize; y++)
            {
                for (int x = 0; x < this._xsize; x++)
                {
                    // ie, making the borders of unwalkable walls
                    if (y == 0 || y == this._ysize - 1 || x == 0 || x == this._xsize - 1)
                    {
                        this.SetCell(x, y, Tile.StoneWall);
                    }
                    else
                    {                        // and fill the rest with dirt
                        this.SetCell(x, y, Tile.Unused);
                    }
                }
            }
        }

        // setting a tile's type
        void SetCell(int x, int y, Tile celltype)
        {
            this._dungeonMap[x + this._xsize * y] = celltype;
        }

        void AddSprinkles()
        {
            // sprinkle out the bonusstuff over the map
            int state = 0; // the state the loop is in, start with the upstairs then downstairs and then to chests
            int chestCount = 0;
            while (state != 10)
            {
                for (int testing = 0; testing < 1000; testing++)
                {
                    var newx = this.GetRand(1, this._xsize - 1);
                    int newy = this.GetRand(1, this._ysize - 2);

                    int ways = 4; // from how many directions we can reach the random spot from

                    // check if we can reach the spot
                    if (GetCellType(newx, newy + 1) == Tile.DirtFloor || GetCellType(newx, newy + 1) == Tile.Corridor)
                    {
                        // north
                        if (GetCellType(newx, newy + 1) != Tile.Door)
                            ways--;
                    }

                    if (GetCellType(newx - 1, newy) == Tile.DirtFloor || GetCellType(newx - 1, newy) == Tile.Corridor)
                    {
                        // east
                        if (GetCellType(newx - 1, newy) != Tile.Door)
                            ways--;
                    }

                    if (GetCellType(newx, newy - 1) == Tile.DirtFloor || GetCellType(newx, newy - 1) == Tile.Corridor)
                    {
                        // south
                        if (GetCellType(newx, newy - 1) != Tile.Door)
                            ways--;
                    }

                    if (GetCellType(newx + 1, newy) == Tile.DirtFloor || GetCellType(newx + 1, newy) == Tile.Corridor)
                    {
                        // west
                        if (GetCellType(newx + 1, newy) != Tile.Door)
                            ways--;
                    }

                    if (state == 0)
                    {
                        if (ways == 0)
                        {
                            // we're in state 0, so we place the upstairs
                            this.SetCell(newx, newy, Tile.Upstairs);
                            state = 1;
                            break;
                        }
                    }
                    else if (state == 1)
                    {
                        if (ways == 0)
                        {
                            // state 1, place a "downstairs"
                            this.SetCell(newx, newy, Tile.Downstairs);
                            state = 2;
                            break;
                        }
                    }
                    else if (state == 2)
                    {
                        if (ways == 0 && GetCellType(newx, newy) == Tile.DirtFloor)
                        {
                            // Place chests on dirt floor
                            this.SetCell(newx, newy, Tile.Chest);
                            chestCount--;

                            if (chestCount <= 0)
                            {
                                state = 10; // End once all chests are placed
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
