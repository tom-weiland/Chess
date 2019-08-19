using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Chess
{
    public enum PieceColor
    {
        white,
        black
    }
    
    public class Piece
    {
        private Tile _currentTile;
        /// <summary>The tile this piece is currently standing on. Setting this also sets the tile's piece value.</summary>
        public Tile CurrentTile
        {
            get => _currentTile;
            set
            {
                if (_currentTile != null)
                {
                    _currentTile.piece = null; // Remove piece from old tile

                    if (value.piece != null)
                    {
                        // If there's a piece on the tile we are moving to
                        value.piece.Remove();
                    }
                }

                // Add piece to new tile
                _currentTile = value;
                PieceImage.SetValue(Grid.RowProperty, value.Row);
                PieceImage.SetValue(Grid.ColumnProperty, value.Column);
                value.piece = this; // Set the tile's piece value to this piece
            }
        }
        
        /// <summary>The color of this piece.</summary>
        public PieceColor Color { get; set; }
        /// <summary>The visual representation of this piece.</summary>
        public Image PieceImage { get; internal set; }

        /// <summary>Stores the original tile this piece was on. Used to reset the board.</summary>
        private Tile originalTile;

        public Piece(Image pieceImage, PieceColor color, Tile currentTile)
        {
            PieceImage = pieceImage;
            Color = color;
            CurrentTile = currentTile;
            originalTile = currentTile;
        }

        /// <summary>Calculates all valid moves for this piece.</summary>
        /// <param name="simulate">Whether or not to check if making a move will leave the king in check.</param>
        /// <returns>A list of all valid moves.</returns>
        public virtual List<Tile> CalculateValidMoves(bool simulate = true)
        {
            // This is really just here to be overriden by its inheriting classes, so it returns an empty array
            return new List<Tile>();
        }

        /// <summary>Moves the pawn to the given tile, provided that the move is possible.</summary>
        /// <param name="toTile">The tile to move the pawn to.</param>
        public virtual void Move(Tile toTile)
        {
            if (GameManager.PossibleMoves.Contains(toTile))
            {
                GameManager.selectedTile.piece.CurrentTile = toTile; // Move the piece

                GameManager.DeselectTile();

                GameManager.ChangeTurn();
            }
        }

        /// <summary>Simulates a move and ensures it does not leave the king in check.</summary>
        /// <param name="toTile">The tile to simulate a move to.</param>
        /// <param name="simulate">Whether or not to simulate the moves. This should only be true for the piece that will be potentially moved.</param>
        /// <returns>True if the move is allowable.</returns>
        public bool SimulateMove(Tile toTile, bool simulate)
        {
            if (simulate)
            {
                Tile originalTile = CurrentTile; // Save the tile so we can reset the piece after simulating the move
                Piece pieceOnTile = toTile.piece; // Save any piece that was on the tile we are testing
                CurrentTile = toTile;

                switch (GameManager.turn)
                {
                    case PieceColor.white:
                        foreach (Tile tile in GameManager.tiles)
                        {
                            if (tile.ContainsPiece() && tile.piece.Color != PieceColor.white)
                            {
                                // If the tile contains a black piece
                                List<Tile> possibleMoves = tile.piece.CalculateValidMoves(false); // Calculate the piece's possible moves
                                if (possibleMoves.Contains(GameManager.whiteKing.CurrentTile))
                                {
                                    // If one of the piece's possible moves is to take out the king
                                    CurrentTile = originalTile; // Reset the piece to its original tile
                                    if (pieceOnTile != null)
                                    {
                                        pieceOnTile.SwapBackIn(toTile); // Reset the piece that was on the tile we tested
                                    }
                                    return false; // Don't allow this white move because it leaves the king in check
                                }
                            }
                        }
                        CurrentTile = originalTile; // Reset the piece to its original tile
                        if (pieceOnTile != null)
                        {
                            pieceOnTile.SwapBackIn(toTile); // Reset the piece that was on the tile we tested
                        }
                        return true; // Allow this white move

                    case PieceColor.black:
                        foreach (Tile tile in GameManager.tiles)
                        {
                            if (tile.ContainsPiece() && tile.piece.Color != PieceColor.black)
                            {
                                // If the tile contains a white piece
                                List<Tile> possibleMoves = tile.piece.CalculateValidMoves(false); // Calculate the piece's possible moves
                                if (possibleMoves.Contains(GameManager.blackKing.CurrentTile))
                                {
                                    // If one of the piece's possible moves is to take out the king
                                    CurrentTile = originalTile; // Reset the piece to its original tile
                                    if (pieceOnTile != null)
                                    {
                                        pieceOnTile.SwapBackIn(toTile); // Reset the piece that was on the tile we tested
                                    }
                                    return false; // Don't allow this black move because it leaves the king in check
                                }
                            }
                        }
                        CurrentTile = originalTile; // Reset the piece to its original tile
                        if (pieceOnTile != null)
                        {
                            pieceOnTile.SwapBackIn(toTile); // Reset the piece that was on the tile we tested
                        }
                        return true; // Allow this black move
                        
                }

                CurrentTile = originalTile; // Reset the piece to its original tile
                if (pieceOnTile != null)
                {
                    pieceOnTile.SwapBackIn(toTile); // Reset the piece that was on the tile we tested
                }
                return false; // This *should* never happen because we've covered all cases in the switch statement, it's just here to satisfy the compiler
            }
            else
            {
                // If we don't want to simulate the move
                return true; // Allow the move
            }
        }

        /// <summary>Removes the piece from the board and moves it to prison.</summary>
        protected virtual void Remove()
        {
            CurrentTile.piece = null;
            
            if (Color == PieceColor.white)
            {
                // Move to black prison
                foreach (Tile tile in GameManager.blackPrison)
                {
                    if (!tile.ContainsPiece())
                    {
                        // If the tile is empty
                        GameManager.page.MoveToPrison(this); // Move the visual representation of the piece to the black prison's grid
                        CurrentTile = tile; // Move the actual piece
                        break; // Stop checking the other prison tiles
                    }
                }
            }
            else
            {
                // Move to white prison
                foreach (Tile tile in GameManager.whitePrison)
                {
                    if (!tile.ContainsPiece())
                    {
                        // If the tile is empty
                        GameManager.page.MoveToPrison(this); // Move the visual representation of the piece to the white prison's grid
                        CurrentTile = tile; // Move the actual piece
                        break; // Stop checking the other prison tiles
                    }
                }
            }
        }

        /// <summary>This is used to retrieve the piece from prison when swapping it for a pawn.</summary>
        public void SwapBackIn()
        {
            CurrentTile.piece = null;
            GameManager.selectedTile.piece.Remove(); // Remove the pawn
            CurrentTile = GameManager.selectedTile;
            GameManager.swappingPiece = false;
            GameManager.DeselectTile();

            // Change whose turn it is
            GameManager.ChangeTurn();
        }
        /// <summary>This is only used reset pieces to their original tile when simulating a move.</summary>
        /// <param name="toTile">The tile to reset the piece to.</param>
        public void SwapBackIn(Tile toTile)
        {
            GameManager.page.MoveToBoard(this);
            CurrentTile.piece = null;
            CurrentTile = toTile;
            PieceImage.Visibility = Visibility.Visible; // Make sure piece is visible, mostly applies to pawns
        }

        /// <summary>Checks all possible moves in a certain direction. This method can be used for all pieces, EXCEPT for Pawns and Knights, since their movement is special.</summary>
        /// <param name="currentTile">The tile on which the piece is currently on.</param>
        /// <param name="forward">Which way is forward for the piece. White = -1. Black = 1.</param>
        /// <param name="direction">Which direction to check.</param>
        /// <param name="maxDistance">How far in the given direction to check.</param>
        public virtual List<Tile> CheckMoves(Tile currentTile, MoveDirection direction, int maxDistance, bool simulate)
        {
            List<Tile> validTiles = new List<Tile>();

            switch (direction)
            {
                case MoveDirection.straight:
                    /* FORWARD */
                    for (int i = 1; i <= maxDistance; i++) // Note that this for loop is NOT zero based!
                    {
                        if (CheckMove(currentTile.Row - i, currentTile.Column, simulate, ref validTiles))
                        {
                            break;
                        }
                    }

                    /* BACKWARD */
                    for (int i = 1; i <= maxDistance; i++) // Note that this for loop is NOT zero based!
                    {
                        if (CheckMove(currentTile.Row + i, currentTile.Column, simulate, ref validTiles))
                        {
                            break;
                        }
                    }

                    /* RIGHT */
                    for (int i = 1; i <= maxDistance; i++) // Note that this for loop is NOT zero based!
                    {
                        if (CheckMove(currentTile.Row, currentTile.Column + i, simulate, ref validTiles))
                        {
                            break;
                        }
                    }

                    /* LEFT */
                    for (int i = 1; i <= maxDistance; i++) // Note that this for loop is NOT zero based!
                    {
                        if (CheckMove(currentTile.Row, currentTile.Column - i, simulate, ref validTiles))
                        {
                            break;
                        }
                    }
                    break;

                case MoveDirection.diagonal:
                    /* FORWARD-RIGHT */
                    for (int i = 1; i <= maxDistance; i++) // Note that this for loop is NOT zero based!
                    {
                        if (CheckMove(currentTile.Row - i, currentTile.Column + i, simulate, ref validTiles))
                        {
                            break;
                        }
                    }

                    /* FORWARD-LEFT */
                    for (int i = 1; i <= maxDistance; i++) // Note that this for loop is NOT zero based!
                    {
                        if (CheckMove(currentTile.Row - i, currentTile.Column - i, simulate, ref validTiles))
                        {
                            break;
                        }
                    }

                    /* BACKWARD-RIGHT */
                    for (int i = 1; i <= maxDistance; i++) // Note that this for loop is NOT zero based!
                    {
                        if (CheckMove(currentTile.Row + i, currentTile.Column + i, simulate, ref validTiles))
                        {
                            break;
                        }
                    }

                    /* BACKWARD-LEFT */
                    for (int i = 1; i <= maxDistance; i++) // Note that this for loop is NOT zero based!
                    {
                        if (CheckMove(currentTile.Row + i, currentTile.Column - i, simulate, ref validTiles))
                        {
                            break;
                        }
                    }
                    break;
            }

            return validTiles;
        }

        /// <summary>Checks a tile to see if it is valid for a move.</summary>
        /// <param name="row">The row of the tile to check.</param>
        /// <param name="column">The column of the tile to check.</param>
        /// <param name="validTiles">The list to add the tile to if it's valid.</param>
        /// <returns>Whether or not to break out of the loop.</returns>
        private bool CheckMove(int row, int column, bool simulate, ref List<Tile> validTiles)
        {
            if (MoveIsOnBoard(row, column))
            {
                // If the proposed move is actually on the board
                Tile tile = GameManager.tiles[row, column];
                if (tile.ContainsPiece())
                {
                    // If the tile contains a piece
                    if (tile.piece.Color != Color && SimulateMove(tile, simulate))
                    {
                        // If the piece is of the opposite color AND the move does not leave the king in check
                        validTiles.Add(tile); // Add the tile to the possible moves
                    }

                    return true; // Stop checking further tiles in this direction
                }
                else
                {
                    // If the tile does not contain a piece
                    if (SimulateMove(tile, simulate))
                    {
                        // If the move does not leave the king in check
                        validTiles.Add(tile); // Add the tile to the possible moves
                    }
                    return false; // Continue checking further tiles in this direction
                }
            }

            return true; // Stop checking further tiles in this direction
        }
        
        /// <summary>Resets the piece to its starting position.</summary>
        public void Reset()
        {
            GameManager.page.MoveToBoard(this); // Ensure the piece is not in jail
            if (originalTile.ContainsPiece() && originalTile.piece != this)
            {
                // Reset any piece that's on the original tile ONLY IF it's a different piece than this one. Even
                // though we call the Reset() method on each piece, this is required to ensure all pieces are
                // *properly* reset. Without this, problems occur.
                originalTile.piece.Reset();
            }
            CurrentTile = originalTile; // Reset the tile it's standing on
            PieceImage.Visibility = Visibility.Visible;
        }

        /// <summary>Checks if the tile at the given row and column is on the board.</summary>
        /// <param name="row">The row of the tile.</param>
        /// <param name="column">The column of the tile.</param>
        /// <returns>True if the move is on the board.</returns>
        public static bool MoveIsOnBoard(int row, int column)
        {
            if (row < 0 || row > 7 || column < 0 || column > 7)
            {
                return false;
            }

            return true;
        }
    }
}
