using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Chess
{
    class Queen : Piece
    {
        // Constructor just calls the base class constructor
        public Queen(Image pieceImage, PieceColor color, Tile currentTile) : base(pieceImage, color, currentTile) { }

        /// <summary>Calculates all valid moves for this piece.</summary>
        /// <param name="simulate">Whether or not to check if making a move will leave the king in check.</param>
        /// <returns>A list of all valid moves.</returns>
        public override List<Tile> CalculateValidMoves(bool simulate = true)
        {
            List<Tile> validTiles = new List<Tile>();

            validTiles.AddRange(CheckMoves(CurrentTile, MoveDirection.straight, 8, simulate)); // Allow queens to move 8 tiles straight
            validTiles.AddRange(CheckMoves(CurrentTile, MoveDirection.diagonal, 8, simulate)); // Allow queens to move 8 tiles diagonally
            return validTiles;
        }
    }
}
