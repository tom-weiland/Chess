using Windows.UI.Xaml.Controls;

namespace Chess
{
    public class Tile
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        /// <summary>The piece occupying this tile. Null if no piece.</summary>
        public Piece piece;
        public Image Highlight { get; private set; }

        public Tile(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public Tile(int row, int column, Image highlight)
        {
            Row = row;
            Column = column;
            Highlight = highlight;
        }
        
        public void SetPiece(Piece piece)
        {
            this.piece = piece;
        }

        public bool ContainsPiece()
        {
            if (piece != null)
            {
                return true;
            }

            return false;
        }
    }
}
