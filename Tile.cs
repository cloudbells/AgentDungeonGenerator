namespace AgentDungeonGenerator
{
    /// <summary>
    /// Represents a single tile within the dungeon.
    /// </summary>
    internal class Tile
    {
        /// <summary>
        /// The tile's type.
        /// </summary>
        public TileType Type { get; set; }
        /// <summary>
        /// The tile's column.
        /// </summary>
        public int Col { get; set; }
        /// <summary>
        /// The tile's row.
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// The index of the tile based on the row and column (increasing from top left to bottom right in the dungeon).
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Constructs a tile with the given type with the given column and row indeces.
        /// </summary>
        /// <param name="type">The type of tile</param>
        /// <param name="col">The column</param>
        /// <param name="row">The row</param>
        public Tile(TileType type, int col, int row)
        {
            Type = type;
            Col = col;
            Row = row;
        }

        public override string ToString()
        {
            return "(" + Col + ", " + Row + ")";
        }
    }

    /// <summary>
    /// The different types of tiles.
    /// </summary>
    internal enum TileType
    {
        Void,
        Room,
        Corridor,
    }
}
