using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Chess
{
    class Pawn : Piece
    {
        public Pawn(Image pieceImage, PieceColor color, Tile currentTile) : base(pieceImage, color, currentTile) { } // Constructor just calls the base class constructor

        /// <summary>Calculates all valid moves for this piece.</summary>
        /// <param name="simulate">Whether or not to check if making a move will leave the king in check.</param>
        /// <returns>A list of all valid moves.</returns>
        public override List<Tile> CalculateValidMoves(bool simulate = true)
        {
            List<Tile> validTiles = new List<Tile>();

            // Pawns can only move forward, which is dependent on its color
            switch (Color)
            {
                case PieceColor.white:
                    if (MoveIsOnBoard(CurrentTile.Row - 1, CurrentTile.Column))
                    {
                        Tile forward1 = GameManager.tiles[CurrentTile.Row - 1, CurrentTile.Column];
                        if (!forward1.ContainsPiece())
                        {
                            // If the tile in front has nothing on it
                            if (SimulateMove(forward1, simulate))
                            {
                                // If the move does not leave the king in check
                                validTiles.Add(forward1); // Forward 1
                            }

                            if (MoveIsOnBoard(CurrentTile.Row - 2, CurrentTile.Column))
                            {
                                Tile forward2 = GameManager.tiles[CurrentTile.Row - 2, CurrentTile.Column];
                                if (CurrentTile == GameManager.tiles[6, CurrentTile.Column] && !forward2.ContainsPiece() && SimulateMove(forward2, simulate))
                                {
                                    // If the pawn is standing on its original tile and the tile 2 in front has nothing on it AND the move does not leave the king in check
                                    validTiles.Add(forward2); // Forward 2
                                }
                            }
                        }
                    }
                    
                    if (MoveIsOnBoard(CurrentTile.Row - 1, CurrentTile.Column + 1))
                    {
                        Tile forward1Right1 = GameManager.tiles[CurrentTile.Row - 1, CurrentTile.Column + 1];
                        if (forward1Right1.ContainsPiece() && forward1Right1.piece.Color == PieceColor.black && SimulateMove(forward1Right1, simulate))
                        {
                            // If the tile has a piece of the opposite color AND the move does not leave the king in check
                            validTiles.Add(forward1Right1); // Forward 1, Right 1
                        }
                    }

                    if (MoveIsOnBoard(CurrentTile.Row - 1, CurrentTile.Column - 1))
                    {
                        Tile forward1Left1 = GameManager.tiles[CurrentTile.Row - 1, CurrentTile.Column - 1];
                        if (forward1Left1.ContainsPiece() && forward1Left1.piece.Color == PieceColor.black && SimulateMove(forward1Left1, simulate))
                        {
                            // If the tile has a piece of the opposite color AND the move does not leave the king in check
                            validTiles.Add(forward1Left1); // Forward 1, Left 1
                        }
                    }

                    return validTiles;

                case PieceColor.black:
                    if (MoveIsOnBoard(CurrentTile.Row + 1, CurrentTile.Column))
                    {
                        Tile forward1 = GameManager.tiles[CurrentTile.Row + 1, CurrentTile.Column];
                        if (!forward1.ContainsPiece())
                        {
                            // If the tile in front has nothing on it
                            if (SimulateMove(forward1, simulate))
                            {
                                // If the move does not leave the king in check
                                validTiles.Add(forward1); // Forward 1
                            }

                            if (MoveIsOnBoard(CurrentTile.Row + 2, CurrentTile.Column))
                            {
                                Tile forward2 = GameManager.tiles[CurrentTile.Row + 2, CurrentTile.Column];
                                if (CurrentTile == GameManager.tiles[1, CurrentTile.Column] && !forward2.ContainsPiece() && SimulateMove(forward2, simulate))
                                {
                                    // If the pawn is standing on its original tile and the tile 2 in front has nothing on it AND the move does not leave the king in check
                                    validTiles.Add(forward2); // Forward 2
                                }
                            }
                        }
                    }

                    if (MoveIsOnBoard(CurrentTile.Row + 1, CurrentTile.Column + 1))
                    {
                        Tile forward1Right1 = GameManager.tiles[CurrentTile.Row + 1, CurrentTile.Column + 1];
                        if (forward1Right1.ContainsPiece() && forward1Right1.piece.Color == PieceColor.white && SimulateMove(forward1Right1, simulate))
                        {
                            // If the tile has a piece of the opposite color AND the move does not leave the king in check
                            validTiles.Add(forward1Right1); // Forward 1, Right 1
                        }
                    }

                    if (MoveIsOnBoard(CurrentTile.Row + 1, CurrentTile.Column - 1))
                    {
                        Tile forward1Left1 = GameManager.tiles[CurrentTile.Row + 1, CurrentTile.Column - 1];
                        if (forward1Left1.ContainsPiece() && forward1Left1.piece.Color == PieceColor.white && SimulateMove(forward1Left1, simulate))
                        {
                            // If the tile has a piece of the opposite color AND the move does not leave the king in check
                            validTiles.Add(forward1Left1); // Forward 1, Left 1
                        }
                    }

                    return validTiles;
                default:
                    // This *should* never happen, because we've covered all color cases, but we need it to satisfy the
                    // compiler. IF this method were actually capable of returning null, we would need to be prepared
                    // to handle that wherever we call it.
                    return null;
            }
        }

        /// <summary>Removes the pawn from the board.</summary>
        protected override void Remove()
        {
            PieceImage.Visibility = Visibility.Collapsed; // Hide the pawn
        }

        /// <summary>Moves the pawn to the given tile, provided that the move is possible.</summary>
        /// <param name="toTile">The tile to move the pawn to.</param>
        public override void Move(Tile toTile)
        {
            if (GameManager.PossibleMoves.Contains(toTile))
            {
                // If the move is possible
                GameManager.selectedTile.piece.CurrentTile = toTile; // Move the piece

                GameManager.DeselectTile();

                bool isWhite = Color == PieceColor.white;
                if (CurrentTile.Row == (isWhite ? 0 : 7))
                {
                    // If pawn is on the last row, meaning the player gets to swap it out for a piece
                    GameManager.SelectTile(CurrentTile, false); // Select the tile the pawn is standing on without calculating moves
                    GameManager.turnText.Text = "CHOOSE A " + (isWhite ? "WHITE" : "BLACK") + " PIECE TO BRING BACK IN";
                    GameManager.swappingPiece = true; // Allow the player to click the prison tiles
                }
                else
                {
                    // Change whose turn it is
                    GameManager.ChangeTurn();
                }
            }
        }
    }
}
