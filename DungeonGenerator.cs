using System;
using System.Linq;

namespace AgentDungeonGenerator
{
    /// <summary>
    /// A dungeon generator.
    /// </summary>
    internal class DungeonGenerator
    {
        public static readonly int DUNGEON_SIZE = 25;
        private readonly int MIN_SIZE = 3;
        private readonly int MAX_SIZE = 7;
        private readonly Random rand;
        private Dungeon dungeon;
        private int agentCol;
        private int agentRow;

        /// <summary>
        /// Constructs a new DungeonGenerator.
        /// </summary>
        /// <param name="display">The display</param>
        public DungeonGenerator(Display display)
        {
            rand = new Random();
            dungeon = new Dungeon(display);
        }

        /// <summary>
        /// Returns a random direction. If a direction is given, it is guaranteed to be different.
        /// </summary>
        /// <param name="direction">The direction <b>NOT</b> to generate</param>
        /// <returns>a randomized direction</returns>
        private Direction RandomizeDirection(Direction? direction)
        {
            Direction[] values = (Direction[])Enum.GetValues(typeof(Direction));
            values = values.Where(val => val != direction).ToArray();
            return values[rand.Next(values.Length)];
        }

        /// <summary>
        /// Returns all <see cref="Tile"/>s within the specified range.<br />
        /// </summary>
        /// <param name="col1">The first column</param>
        /// <param name="col2">The second column</param>
        /// <param name="row1">The first row</param>
        /// <param name="row2">The second row</param>
        /// <returns>all tiles in the specified range</returns>
        private Tile[,] GetAllDungeonTilesByRowCol(int col1, int col2, int row1, int row2)
        {
            if (col1 > col2)
            {
                (col2, col1) = (col1, col2);
            }
            if (row1 > row2)
            {
                (row2, row1) = (row1, row2);
            }
            Tile[,] tiles = new Tile[(col2 - col1 + 1), (row2 - row1 + 1)];
            int colIndex = 0;
            for (int col = col1; col <= col2; col++)
            {
                int rowIndex = 0;
                for (int row = row1; row <= row2; row++)
                {
                    tiles[colIndex, rowIndex] = dungeon.Tiles[col, row];
                    rowIndex++;
                }
                colIndex++;
            }
            return tiles;
        }

        /// <summary>
        /// Attempts to add a room at the given location. Returns 0 if unsuccessful, >0 otherwise.
        /// </summary>
        /// <param name="agentCol">Current column of the agent</param>
        /// <param name="agentRow">Current row of the agent</param>
        /// <returns>0 if unsuccessful, >0 otherwise</returns>
        private int AddRoom()
        {
            int originalWidth = rand.Next(MIN_SIZE, MAX_SIZE + 1);
            int originalHeight = rand.Next(MIN_SIZE, MAX_SIZE + 1);
            // If the randomized dimension doesn't work, there is no point trying bigger sizes.
            // Idea: is it worth randomizing whether we reduce height or width first?
            for (int width = originalWidth; width >= MIN_SIZE; width--)
            {
                for (int height = originalHeight; height >= MIN_SIZE; height--)
                {
                    int col1, col2, row1, row2;
                    bool isIntersecting = false;
                    if (originalWidth % 2 == 0) // If even the room will be one tile further to the left than the right. Maybe also worth randomizing this?
                    {
                        col1 = agentCol - width / 2;
                        col2 = agentCol + width / 2 - 1;
                    }
                    else // If odd.
                    {
                        col1 = agentCol - (width - 1) / 2;
                        col2 = agentCol + (width - 1) / 2;
                    }
                    if (height % 2 == 0)
                    {
                        row1 = agentRow - height / 2;
                        row2 = agentRow + height / 2 - 1;
                    }
                    else
                    {
                        row1 = agentRow - (height - 1) / 2;
                        row2 = agentRow + (height - 1) / 2;
                    }
                    // Make sure we're not outside the edges.
                    if (col1 < 1 || col2 > DUNGEON_SIZE - 2 || row1 < 1 || row2 > DUNGEON_SIZE - 2)
                    {
                        continue;
                    }
                    // Check if this room will intersect with another room.
                    foreach (Tile tile in GetAllDungeonTilesByRowCol(col1 - 1, col2 + 1, row1 - 1, row2 + 1))
                    {
                        if (tile.Type == TileType.Room)
                        {
                            isIntersecting = true;
                            break;
                        }
                    }
                    if (!isIntersecting)
                    {
                        // Add the room.
                        foreach (Tile roomTile in GetAllDungeonTilesByRowCol(col1, col2, row1, row2))
                        {
                            roomTile.Type = TileType.Room;
                        }
                        return height * width;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns the size of the current room in the given direction.
        /// </summary>
        /// <param name="agentCol">The current column of the agent</param>
        /// <param name="agentRow">The current row of the agent</param>
        /// <param name="direction">The direction</param>
        /// <returns>the size of the room in the given direction</returns>
        private int GetRoomOffset(Direction direction)
        {
            int hAdd = 0;
            int vAdd = 0;
            switch (direction)
            {
                case Direction.North:
                    hAdd = 0;
                    vAdd = -1;
                    break;
                case Direction.East:
                    hAdd = 1;
                    vAdd = 0;
                    break;
                case Direction.South:
                    hAdd = 0;
                    vAdd = 1;
                    break;
                case Direction.West:
                    hAdd = -1;
                    vAdd = 0;
                    break;
            }
            // If we are on the edge already.
            if (agentCol + hAdd < 0 || agentCol + hAdd > DUNGEON_SIZE - 1 || agentRow + vAdd < 0 || agentRow + vAdd > DUNGEON_SIZE - 1)
            {
                return 0;
            }
            int offset = 0;
            int col = hAdd;
            int row = vAdd;
            Tile tile = dungeon.Tiles[agentCol + col, agentRow + row];
            while (tile.Type == TileType.Room)
            {
                offset++;
                col += hAdd;
                row += vAdd;
                tile = dungeon.Tiles[agentCol + col, agentRow + row];
            }
            return offset;
        }

        /// <summary>
        /// Generates a corridor in the given direction from the given x- and y-coordinates if possible.
        /// </summary>
        /// <param name="direction">The desired direction of the corridor</param>
        /// <param name="agentCol">The current x-coordinate of the agent</param>
        /// <param name="agentRow">The current y-coordinate of the agent</param>
        /// <returns>(length, tiles) if successful, (0, null) otherwise</returns>
        private (int, Tile[,]) GenerateCorridor(Direction direction)
        {
            int startingLength = rand.Next(MIN_SIZE, MAX_SIZE + 1);
            for (int length = startingLength; length >= MIN_SIZE; length--)
            {
                int row1, row2, col1, col2;
                row1 = row2 = col1 = col2 = 0;
                int checkRow1, checkRow2, checkCol1, checkCol2; // The tiles to check.
                checkRow1 = checkRow2 = checkCol1 = checkCol2 = 0;
                int hOffset = 0;
                int vOffset = 0;
                switch (direction)
                {
                    case Direction.North:
                        if (agentRow - MIN_SIZE < 1)
                        {
                            return (0, null);
                        }
                        hOffset = 0;
                        vOffset = -GetRoomOffset(Direction.North) - 1; // This tile is where the void between rooms starts.
                        checkCol1 = agentCol - 1;
                        checkCol2 = agentCol + 1;
                        checkRow1 = agentRow - length - 1;
                        checkRow2 = agentRow;
                        col1 = col2 = agentCol;
                        row1 = agentRow - length;
                        row2 = agentRow;
                        break;
                    case Direction.East:
                        if (agentCol + MIN_SIZE > DUNGEON_SIZE - 2)
                        {
                            return (0, null);
                        }
                        hOffset = GetRoomOffset(Direction.East) + 1;
                        vOffset = 0;
                        checkCol1 = agentCol;
                        checkCol2 = agentCol + length + 1;
                        checkRow1 = agentRow - 1;
                        checkRow2 = agentRow + 1;
                        col1 = agentCol;
                        col2 = agentCol + length;
                        row1 = row2 = agentRow;
                        break;
                    case Direction.South:
                        if (agentRow + MIN_SIZE > DUNGEON_SIZE - 2)
                        {
                            return (0, null);
                        }
                        hOffset = 0;
                        vOffset = GetRoomOffset(Direction.South) + 1;
                        checkCol1 = agentCol - 1;
                        checkCol2 = agentCol + 1;
                        checkRow1 = agentRow;
                        checkRow2 = agentRow + length + 1;
                        col1 = col2 = agentCol;
                        row1 = agentRow;
                        row2 = agentRow + length;
                        break;
                    case Direction.West:
                        if (agentCol - MIN_SIZE < 1)
                        {
                            return (0, null);
                        }
                        hOffset = -GetRoomOffset(Direction.West) - 1;
                        vOffset = 0;
                        checkCol1 = agentCol - length - 1;
                        checkCol2 = agentCol;
                        checkRow1 = agentRow - 1;
                        checkRow2 = agentRow + 1;
                        col1 = agentCol - length;
                        col2 = agentCol;
                        row1 = row2 = agentRow;
                        break;
                }
                // Check if the corridor is out of bounds or intersects something.
                if (checkCol1 + hOffset < 0 || checkCol2 + hOffset > DUNGEON_SIZE - 1 || checkRow1 + vOffset < 0 || checkRow2 + vOffset > DUNGEON_SIZE - 1)
                {
                    continue;
                }
                Tile[,] tiles = GetAllDungeonTilesByRowCol(checkCol1 + hOffset, checkCol2 + hOffset, checkRow1 + vOffset, checkRow2 + vOffset);
                dungeon.Print(agentCol, agentRow, 100, 100, tiles); // For debug.
                foreach (Tile tile in tiles)
                {
                    if (tile == null || tile.Type != TileType.Void)
                    {
                        return (0, null);
                    }
                }
                // Don't add corridor on the edge.
                if (col1 + hOffset < 1 || col2 + hOffset > DUNGEON_SIZE - 2 || row1 + vOffset < 1 || row2 + vOffset > DUNGEON_SIZE - 2)
                {
                    continue;
                }
                tiles = GetAllDungeonTilesByRowCol(col1 + hOffset, col2 + hOffset, row1 + vOffset, row2 + vOffset);
                return (tiles.Length + Math.Abs(hOffset) + Math.Abs(vOffset), tiles);
            }
            return (0, null);
        }

        /// <summary>
        /// Attempts to add a corridor at the given location. Returns (direction, length) if successful, (default, 0) otherwise.
        /// </summary>
        /// <param name="agentCol">The current column of the agent</param>
        /// <param name="agentRow">The current row of the agent</param>
        /// <returns>(direction, length) if successful, (default, 0) otherwise</returns>
        private (Direction, int) AddCorridor()
        {
            Direction[] values = (Direction[])Enum.GetValues(typeof(Direction));
            // Prioritize length over direction.
            while (values.Length > 0) {
                // Randomize direction until there are no more directions.
                Direction direction = values[rand.Next(values.Length)];
                values = values.Where(val => val != direction).ToArray();
                (int length, Tile[,] tiles) = GenerateCorridor(direction);
                if (tiles != null && tiles.Length > MIN_SIZE) {
                    foreach (Tile tile in tiles)
                    {
                        tile.Type = TileType.Corridor;
                    }
                    return (direction, length - 1);
                }
            }
            return (default, 0);
        }

        /// <summary>
        /// Generates a dungeon using an agent more constructively than stochastically.
        /// </summary>
        public void GenerateDungeon()
        {
            Tile[,] tiles = dungeon.Tiles;
            agentCol = rand.Next(DUNGEON_SIZE);
            agentRow = rand.Next(DUNGEON_SIZE);
            dungeon.Print(agentCol, agentRow, 100, 100, null);
            int failedCounter = 0;
            while (failedCounter < 2)
            {
                int size = AddRoom();
                if (size > 0)
                {
                    failedCounter = 0;
                    Console.WriteLine("Added a ROOM with area " + size + " at " + "(" + agentCol + ", " + agentRow + ") [tile #" + (agentRow * DUNGEON_SIZE + agentCol) + "]");
                } else
                {
                    failedCounter++;
                    Console.WriteLine("Could not add a ROOM");
                }
                dungeon.Print(agentCol, agentRow, 100, 100, null);
                (Direction direction, int length) = AddCorridor();
                if (length > 0) {
                    failedCounter = 0;
                    switch (direction)
                    {
                        case Direction.North:
                            agentRow -= length;
                            break;
                        case Direction.East:
                            agentCol += length;
                            break;
                        case Direction.South:
                            agentRow += length;
                            break;
                        case Direction.West:
                            agentCol -= length;
                            break;
                    }
                    Console.WriteLine("Added a CORRIDOR towards " + direction + " at: (" + agentCol + ", " + agentRow + ") with length " + length + " [tile #" + (agentRow * DUNGEON_SIZE + agentCol) + "]");
                } else
                {
                    failedCounter++;
                    Console.WriteLine("Could not add a CORRIDOR");
                }
                dungeon.Print(agentCol, agentRow, 100, 100, null);
            }
            Console.WriteLine("Done! Unable to add another room or corridor.");
            dungeon.PrintFinal();
        }

        /// <summary>
        /// Generates a dungeon by completely random walking.
        /// </summary>
        public void GenerateDungeonStochastic()
        {
            double ACCEPTABLE_SCORE = 0.4; // Percentage that needs to be filled.
            int ROOM_CHANCE_INCREASE = 2;
            int DIRECTION_CHANCE_INCREASE = 2;
            Tile[,] tiles = dungeon.Tiles;
            agentCol = rand.Next(DUNGEON_SIZE);
            agentRow = rand.Next(DUNGEON_SIZE);
            Tile tile = dungeon.Tiles[agentCol, agentRow];
            tile.Type = TileType.Corridor;
            Console.WriteLine("Adding a CORRIDOR at: (" + agentCol + ", " + agentRow + ") [tile #" + (agentRow * DUNGEON_SIZE + agentCol) + "]");
            int filledTiles = 1;
            int directionChance = DIRECTION_CHANCE_INCREASE;
            int roomChance = ROOM_CHANCE_INCREASE;
            Direction direction = RandomizeDirection(null);
            dungeon.Print(agentCol, agentRow, directionChance, roomChance, null);
            while (filledTiles < ACCEPTABLE_SCORE * DUNGEON_SIZE * DUNGEON_SIZE)
            {
                // Roll a change of direction.
                if (rand.Next(100) < directionChance)
                {
                    directionChance = 0;
                    direction = RandomizeDirection(direction);
                    Console.WriteLine("Changed direction to: " + direction);
                }
                else
                {
                    directionChance += DIRECTION_CHANCE_INCREASE;
                }
                // Roll a room.
                if (rand.Next(100) < roomChance)
                {
                    int size = AddRoom();
                    if (size > 0)
                    {
                        roomChance = 0;
                        filledTiles += size;
                        Console.WriteLine("Added a ROOM with area " + size + " at " + "(" + agentCol + ", " + agentRow + ") [tile #" + (agentRow * DUNGEON_SIZE + agentCol) + "]");
                    }
                    else
                    {
                        roomChance += ROOM_CHANCE_INCREASE;
                    }
                }
                else
                {
                    roomChance += ROOM_CHANCE_INCREASE;
                }
                int col = 0;
                int row = 0;
                switch (direction)
                {
                    case Direction.North:
                        if (agentRow == 0)
                        {
                            direction = RandomizeDirection(direction);
                            break;
                        }
                        row = -1;
                        break;
                    case Direction.East:
                        if (agentCol == DUNGEON_SIZE - 1)
                        {
                            direction = RandomizeDirection(direction);
                            break;
                        }
                        col = 1;
                        break;
                    case Direction.South:
                        if (agentRow == DUNGEON_SIZE - 1)
                        {
                            direction = RandomizeDirection(direction);
                            break;
                        }
                        row = 1;
                        break;
                    case Direction.West:
                        if (agentCol == 0)
                        {
                            direction = RandomizeDirection(direction);
                            break;
                        }
                        col = -1;
                        break;
                }
                agentCol += col;
                agentRow += row;
                tile = dungeon.Tiles[agentCol, agentRow];
                if (tile.Type == TileType.Void)
                {
                    filledTiles++;
                    tile.Type = TileType.Corridor;
                    Console.WriteLine("Added a CORRIDOR at: (" + agentCol + ", " + agentRow + ") [tile #" + (agentRow * DUNGEON_SIZE + agentCol) + "]");
                }
                dungeon.Print(agentCol, agentRow, directionChance, roomChance, null);
            };
            Console.WriteLine("DONE! Filled " + ACCEPTABLE_SCORE * 100 + "% of the dungeon!");
            dungeon.PrintFinal();
        }
    }

    /// <summary>
    /// Current direction of the agent.
    /// </summary>
    internal enum Direction
    {
        North,
        East,
        South,
        West
    }
}
