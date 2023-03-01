namespace AgentDungeonGenerator
{
    /// <summary>
    /// Represents a dungeon.
    /// </summary>
    internal class Dungeon
    {
        private readonly Display display;
        /// <summary>
        /// The tiles in the dungeon.
        /// </summary>
        public Tile[,] Tiles { get; private set; }

        /// <summary>
        /// Constructs an empty dungeon.
        /// </summary>
        /// <param name="display"></param>
        public Dungeon(Display display)
        {
            this.display = display;
            Tiles = new Tile[DungeonGenerator.DUNGEON_SIZE, DungeonGenerator.DUNGEON_SIZE];
            for (int col = 0; col < DungeonGenerator.DUNGEON_SIZE; col++)
            {
                for (int row = 0; row < DungeonGenerator.DUNGEON_SIZE; row++)
                {
                    Tiles[col, row] = new Tile(TileType.Void, col, row);
                }
            }
        }

        /// <summary>
        /// Prints the dungeon.
        /// </summary>
        /// <param name="agentCol">The current column of the agent</param>
        /// <param name="agentRow">The current row of the agent</param>
        /// <param name="directionChance">The current chance to change direction</param>
        /// <param name="roomChance">The current chance to add a room</param>
        public void Print(int agentCol, int agentRow, int directionChance, int roomChance, Tile[,] debugTiles)
        {
            display.Tiles = Tiles;
            display.AgentLocation = (agentCol, agentRow);
            display.DirectionChance = directionChance;
            display.RoomChance = roomChance;
            display.DebugTiles = debugTiles;
            void action () => display.Refresh();
            action();
        }

        /// <summary>
        /// Prints the final dungeon. Call this when the generator is done generating.
        /// </summary>
        public void PrintFinal()
        {
            display.Complete = true;
            void action() => display.Refresh();
            action();
        }
    }
}
