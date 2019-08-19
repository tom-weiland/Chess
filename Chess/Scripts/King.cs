using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Chess
{
    class King : Piece
    {
        // Constructor just calls the base class constructor
        public King(Image pieceImage, PieceColor color, Tile currentTile) : base(pieceImage, color, currentTile) { }

        /// <summary>Calculates all valid moves for this piece.</summary>
        /// <param name="simulate">Whether or not to check if making a move will leave the king in check.</param>
        /// <returns>A list of all valid moves.</returns>
        public override List<Tile> CalculateValidMoves(bool simulate = true)
        {
            List<Tile> validTiles = new List<Tile>();

            validTiles.AddRange(CheckMoves(CurrentTile, MoveDirection.straight, 1, simulate)); // Allow kings to move 1 tile straight
            validTiles.AddRange(CheckMoves(CurrentTile, MoveDirection.diagonal, 1, simulate)); // Allow kings to move 1 tile diagonally
            return validTiles;
        }
    }
}
