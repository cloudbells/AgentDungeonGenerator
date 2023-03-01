using System.Drawing;
using System.Windows.Forms;

namespace AgentDungeonGenerator
{
    /// <summary>
    /// The main window/display for the dungeon.
    /// </summary>
    public partial class Display : Form
    {
        internal Tile[,] Tiles { get; set; }
        internal Tile[,] DebugTiles { get; set; }
        internal (int Col, int Row) AgentLocation { get; set; }
        internal int DirectionChance { get; set; }
        internal int RoomChance { get; set; }
        internal bool Complete { get; set; }

        public Display()
        {
            InitializeComponent();
            Complete = false;
        }

        /// <summary>
        /// Called when the display is first shown (finished loading).
        /// </summary>
        private void Display_Shown(object sender, System.EventArgs e)
        {
            new DungeonGenerator(this).GenerateDungeon();
        }

        /// <summary>
        /// (Re-)Paints the dungeon.
        /// </summary>
        private void mainCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush agentBrush = new SolidBrush(Color.Red);
            Brush corridorBrush = new SolidBrush(Color.White);
            Brush roomBrush = new SolidBrush(Color.Gray);
            Brush debugBrush = new SolidBrush(Color.Green);
            Pen pen = new Pen(corridorBrush);
            float width = mainCanvas.ClientSize.Width - 1;
            float height = mainCanvas.ClientSize.Height - 1;
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, width, height);
            float rectWidth = width / DungeonGenerator.DUNGEON_SIZE;
            float rectHeight = (float) height / DungeonGenerator.DUNGEON_SIZE;
            if (Tiles != null)
            {
                foreach (Tile tile in Tiles)
                {
                    if (Complete)
                    {
                        if (tile.Type != TileType.Void)
                        {
                            g.FillRectangle(corridorBrush, (tile.Col * rectWidth), tile.Row * rectHeight, rectWidth, rectHeight);
                        }
                    } else
                    {
                        if (tile.Type == TileType.Room)
                        {
                            g.FillRectangle(roomBrush, (tile.Col * rectWidth), tile.Row * rectHeight, rectWidth, rectHeight);
                        }
                        if (tile.Type == TileType.Corridor)
                        {
                            g.FillRectangle(corridorBrush, (tile.Col * rectWidth), tile.Row * rectHeight, rectWidth, rectHeight);
                        }
                        if (AgentLocation.Col == tile.Col && AgentLocation.Row == tile.Row)
                        {
                            g.FillRectangle(agentBrush, (tile.Col * rectWidth), tile.Row * rectHeight, rectWidth, rectHeight);
                        }
                    }
                }
            }
            if (!Complete)
            {
                directionChanceLabel.Text = "Chance to change direction: " + DirectionChance + "%";
                roomChanceLabel.Text = "Chance to add a room: " + RoomChance + "%";
                if (DebugTiles != null)
                {
                    foreach (Tile tile in DebugTiles)
                    {
                        if (AgentLocation.Col == tile.Col && AgentLocation.Row == tile.Row)
                        {
                            g.FillRectangle(agentBrush, (tile.Col * rectWidth), tile.Row * rectHeight, rectWidth, rectHeight);
                        }
                        else
                        {
                            g.FillRectangle(debugBrush, (tile.Col * rectWidth), tile.Row * rectHeight, rectWidth, rectHeight);
                        }
                    }
                }
            }
            else
            {
                directionChanceLabel.Text = "";
                roomChanceLabel.Text = "";
            }
        }
    }
}
