using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Chess
{
    class Knight : Piece
    {
        // Constructor just calls the base class constructor
        public Knight(Image pieceImage, PieceColor color, Tile currentTile) : base(pieceImage, color, currentTile) { }

        /// <summary>Calculates all valid moves for this piece.</summary>
        /// <param name="simulate">Whether or not to check if making a move will leave the king in check.</param>
        /// <returns>A list of all valid moves.</returns>
        public override List<Tile> CalculateValidMoves(bool simulate = true)
        {
            List<Tile> validTiles = new List<Tile>();

            CheckMove(CurrentTile.Row - 2, CurrentTile.Column - 1, simulate, ref validTiles); // Allow knights to move forward 2, left 1
            CheckMove(CurrentTile.Row - 2, CurrentTile.Column + 1, simulate, ref validTiles); // Allow knights to move forward 2, right 1
            CheckMove(CurrentTile.Row + 2, CurrentTile.Column - 1, simulate, ref validTiles); // Allow knights to move backward 2, left 1
            CheckMove(CurrentTile.Row + 2, CurrentTile.Column + 1, simulate, ref validTiles); // Allow knights to move backward 2, right 1
            CheckMove(CurrentTile.Row - 1, CurrentTile.Column + 2, simulate, ref validTiles); // Allow knights to move right 2, forward 1
            CheckMove(CurrentTile.Row + 1, CurrentTile.Column + 2, simulate, ref validTiles); // Allow knights to move right 2, backward 1
            CheckMove(CurrentTile.Row - 1, CurrentTile.Column - 2, simulate, ref validTiles); // Allow knights to move left 2, forward 1
            CheckMove(CurrentTile.Row + 1, CurrentTile.Column - 2, simulate, ref validTiles); // Allow knights to move left 2, backward 1

            return validTiles;
        }

        /// <summary>Checks if a move is valid.</summary>
        /// <param name="row">The row of the tile to check.</param>
        /// <param name="column">The column of the tile to check.</param>
        /// <param name="simulate">Whether or not to check if making this move will leave the king in check.</param>
        /// <param name="validTiles">The list of valid tiles to add to if this move is valid.</param>
        private void CheckMove(int row, int column, bool simulate, ref List<Tile> validTiles)
        {
            if (MoveIsOnBoard(row, column))
            {
                Tile tile = GameManager.tiles[row, column];

                if ((!tile.ContainsPiece() || tile.piece.Color != Color) && SimulateMove(tile, simulate))
                {
                    // If the tile doesn't contain a piece or the piece is on the other team AND the move does not leave the king in check
                    validTiles.Add(tile); // Add the tile to the possible moves
                }
            }
        }
    }
}
