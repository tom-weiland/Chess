using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Chess
{
    public enum MoveDirection
    {
        straight,
        diagonal
    }

    class GameManager
    {
        /// <summary>A referene to the MainPage instance of the game.</summary>
        public static MainPage page;
        /// <summary>A reference to the visual highlight for the selected tile.</summary>
        private static Image selectedTileImage;
        /// <summary>The game board's tiles.</summary>
        public static Tile[,] tiles = new Tile[8, 8];
        /// <summary>The white prison's tiles.</summary>
        public static Tile[,] whitePrison = new Tile[4, 2];
        /// <summary>The black prison's tiles.</summary>
        public static Tile[,] blackPrison = new Tile[4, 2];
        /// <summary>The instance of the white king.</summary>
        public static King whiteKing;
        /// <summary>The instance of the black king.</summary>
        public static King blackKing;
        /// <summary>The tile currently selected. Null unless the clicked tile contains a piece.</summary>
        public static Tile selectedTile;
        /// <summary>Which color's turn it is.</summary>
        public static PieceColor turn = PieceColor.white; // White goes first
        /// <summary>True if a player is swapping out a pawn for a piece in prison.</summary>
        public static bool swappingPiece = false;
        /// <summary>True if checkmate.</summary>
        public static bool gameOver = false;
        /// <summary>The text block indicating whose turn it is.</summary>
        public static TextBlock turnText;

        private static Tile[] _possibleMoves;
        /// <summary>Stores all possible moves for the currently selected piece. Null if no piece is selected or if no moves are available.</summary>
        public static Tile[] PossibleMoves
        {
            get => _possibleMoves;
            set
            {
                _possibleMoves = value;
                if (value != null)
                {
                    HighlightValidMoves();
                }
            }
        }

        /// <summary></summary>
        /// <param name="mainPage">The MainPage instance.</param>
        /// <param name="selectedTile">The Image instance of the selected tile.</param>
        /// <param name="turn">The TextBlock instance of the turn text.</param>
        public static void Setup(MainPage mainPage, Image selectedTile, TextBlock turn)
        {
            page = mainPage;
            selectedTileImage = selectedTile;
            turnText = turn;
        }

        /// <summary>Selects a tile with a piece on it and calculates valid moves.</summary>
        /// <param name="tile">The tile to select.</param>
        /// <param name="calculateMoves">Whether or not it should highlight possible moves.</param>
        /// <returns>Valid tiles to move to.</returns>
        public static void SelectTile(Tile tile, bool calculateMoves = true)
        {
            DeselectTile(); // Make sure everything is deselected first
            selectedTile = tile;
            selectedTileImage.SetValue(Grid.RowProperty, tile.Row);
            selectedTileImage.SetValue(Grid.ColumnProperty, tile.Column);
            selectedTileImage.Visibility = Visibility.Visible;

            if (calculateMoves)
            {
                PossibleMoves = tile.piece.CalculateValidMoves().ToArray();
            }
        }
        
        /// <summary>Deselects the currently selected tile, un-highlights all highlighted tiles, and clears the PossibleMoves array.</summary>
        public static void DeselectTile()
        {
            selectedTileImage.Visibility = Visibility.Collapsed;
            selectedTile = null;

            if (PossibleMoves != null)
            {
                foreach (Tile tile in PossibleMoves)
                {
                    tile.Highlight.Visibility = Visibility.Collapsed;
                }
            }
            
            PossibleMoves = null;
        }

        /// <summary>Toggles whose turn it is.</summary>
        public static void ChangeTurn()
        {
            switch (turn)
            {
                case PieceColor.white:
                    turn = PieceColor.black;
                    turnText.Text = "BLACK'S TURN";
                    
                    break;
                case PieceColor.black:
                    turn = PieceColor.white;
                    turnText.Text = "WHITE'S TURN";
                    break;
            }
            
            if (IsCheckmate())
            {
                // If checkmate, end the game
                gameOver = true;
                page.EndGame();
            }
        }

        /// <summary>Checks for checkmate.</summary>
        /// <returns>True if checkmate.</returns>
        private static bool IsCheckmate()
        {
            foreach (Tile tile in tiles)
            {
                if (tile.ContainsPiece() && tile.piece.Color == turn)
                {
                    // If the piece on the tile belongs to the player whose turn it is
                    if (tile.piece.CalculateValidMoves().Count > 0)
                    {
                        // If the piece has at least one valid move
                        return false; // No checkmate
                    }
                }
            }

            return true; // None of the player's pieces can make a move, therefore checkmate
        }
        
        /// <summary>Highlights all tiles that the currently selected piece could move to.</summary>
        private static void HighlightValidMoves()
        {
            foreach (Tile tile in PossibleMoves)
            {
                tile.Highlight.Visibility = Visibility.Visible;
            }
        }
    }
}
