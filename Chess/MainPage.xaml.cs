using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Chess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<Piece> pieces = new List<Piece>();

        public MainPage()
        {
            this.InitializeComponent();

            GameManager.Setup(this, SelectedTile, TurnText);
            Setup();
        }

        /// <summary>Performs all initially required setup for the game.</summary>
        public void Setup()
        {
            // Initialize the game board and its tiles
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    // Give each tile a highlight
                    Image highlight = new Image
                    {
                        Source = new BitmapImage(new Uri("ms-appx:///Assets/Game/Tile-Highlight.png", UriKind.Absolute)),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        IsHitTestVisible = false,
                        Visibility = Visibility.Collapsed
                    };

                    Board.Children.Add(highlight);
                    highlight.SetValue(Grid.RowProperty, row);
                    highlight.SetValue(Grid.ColumnProperty, column);

                    GameManager.tiles[row, column] = new Tile(row, column, highlight);
                }
            }

            // Initialize prisons and their tiles
            for (int prisonRow = 0; prisonRow < 4; prisonRow++)
            {
                for (int prisonColumn = 0; prisonColumn < 2; prisonColumn++)
                {
                    GameManager.whitePrison[prisonRow, prisonColumn] = new Tile(prisonRow, prisonColumn);
                    GameManager.blackPrison[prisonRow, prisonColumn] = new Tile(prisonRow, prisonColumn);
                }
            }

            // Place all pieces on board
            for (int color = 0; color < 2; color++)
            {
                bool colorIsWhite = color == PieceColor.white.GetHashCode(); // Assign this so we can use it easily multiple times
                
                // Inline-if         {Condition}   {If true} {If false}
                string colorString = colorIsWhite ? "White" : "Black";


                pieces.Add(new Rook((Image)FindName(colorString + "Rook1"), (PieceColor)color, GameManager.tiles[colorIsWhite ? 7 : 0, 0]));
                pieces.Add(new Rook((Image)FindName(colorString + "Rook2"), (PieceColor)color, GameManager.tiles[colorIsWhite ? 7 : 0, 7]));
                pieces.Add(new Knight((Image)FindName(colorString + "Knight1"), (PieceColor)color, GameManager.tiles[colorIsWhite ? 7 : 0, 1]));
                pieces.Add(new Knight((Image)FindName(colorString + "Knight2"), (PieceColor)color, GameManager.tiles[colorIsWhite ? 7 : 0, 6]));
                pieces.Add(new Bishop((Image)FindName(colorString + "Bishop1"), (PieceColor)color, GameManager.tiles[colorIsWhite ? 7 : 0, 2]));
                pieces.Add(new Bishop((Image)FindName(colorString + "Bishop2"), (PieceColor)color, GameManager.tiles[colorIsWhite ? 7 : 0, 5]));
                pieces.Add(new Queen((Image)FindName(colorString + "Queen"), (PieceColor)color, GameManager.tiles[colorIsWhite ? 7 : 0, colorIsWhite ? 3 : 4]));

                GameManager.whiteKing = new King((Image)FindName("WhiteKing"), PieceColor.white, GameManager.tiles[7, 4]);
                GameManager.blackKing = new King((Image)FindName("BlackKing"), PieceColor.black, GameManager.tiles[0, 3]);
                pieces.Add(GameManager.whiteKing);
                pieces.Add(GameManager.blackKing);

                for (int i = 0; i < 8; i++)
                {
                    //                  Find the correct Pawn's image                 Set pawn instance's color              Set row based on color
                    pieces.Add(new Pawn((Image)FindName(colorString + "Pawn" + (i + 1)), (PieceColor)color, GameManager.tiles[colorIsWhite ? 6 : 1, i]));
                }
            }
        }
        
        public void MoveToPrison(Piece piece)
        {
            if (WhitePrison.Children.Contains(piece.PieceImage) || BlackPrison.Children.Contains(piece.PieceImage))
            {
                // If the piece is already/still in one of the prisons
                return; // Abort
            }

            switch (piece.Color)
            {
                case PieceColor.white:
                    Board.Children.Remove(piece.PieceImage);
                    BlackPrison.Children.Add(piece.PieceImage);
                    break;
                case PieceColor.black:
                    Board.Children.Remove(piece.PieceImage);
                    WhitePrison.Children.Add(piece.PieceImage);
                    break;
            }
        }

        public void MoveToBoard(Piece piece)
        {
            if (Board.Children.Contains(piece.PieceImage))
            {
                // If the piece is already/still on the board
                return; // Abort
            }

            switch (piece.Color)
            {
                case PieceColor.white:
                    BlackPrison.Children.Remove(piece.PieceImage);
                    Board.Children.Add(piece.PieceImage);
                    break;
                case PieceColor.black:
                    WhitePrison.Children.Remove(piece.PieceImage);
                    Board.Children.Add(piece.PieceImage);
                    break;
            }
        }

        public void EndGame()
        {
            switch (GameManager.turn)
            {
                case PieceColor.white:
                    WinText.Text = "Black Wins!";
                    CheckmateText.Foreground = new SolidColorBrush(Colors.Black);
                    WinText.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case PieceColor.black:
                    WinText.Text = "White Wins!";
                    CheckmateText.Foreground = new SolidColorBrush(Colors.White);
                    WinText.Foreground = new SolidColorBrush(Colors.White);
                    break;
            }

            EndScreen.Visibility = Visibility.Visible;
            CheckmateText.Visibility = Visibility.Visible;
            WinText.Visibility = Visibility.Visible;
            RestartButton.Visibility = Visibility.Visible;
            TurnText.Visibility = Visibility.Collapsed;
        }
        
        public void TileClicked(int row, int column)
        {
            if (!GameManager.swappingPiece && !GameManager.gameOver)
            {
                // If neither of the players is currently swapping out a piece
                Tile tile = GameManager.tiles[row, column];
                if (tile.ContainsPiece() && tile.piece.Color == GameManager.turn)
                {
                    // If the clicked tile has a piece on it which belongs to the player whose turn it is
                    if (GameManager.selectedTile != null && GameManager.selectedTile == tile)
                    {
                        // If the clicked tile is already selected
                        GameManager.DeselectTile(); // Deselect it
                    }
                    else
                    {
                        // If the clicked tile isn't already selected
                        GameManager.SelectTile(tile); // Select it
                    }
                }
                else
                {
                    // If the clicked tile has no piece on it OR the piece doesn't belong to the player whose turn it is
                    if (GameManager.selectedTile != null)
                    {
                        // If another tile with a piece is already selected
                        GameManager.selectedTile.piece.Move(tile);
                    }
                }
            }
        }

        /// <summary>Handles clicks on prison tiles.</summary>
        /// <param name="row">The row of the tile clicked.</param>
        /// <param name="column">The column of the tile clicked.</param>
        /// <param name="color">The PRISON's color.</param>
        public void PrisonClicked(int row, int column, PieceColor color)
        {
            if (color != GameManager.turn && GameManager.swappingPiece)
            {
                // If the PRISON'S color does not match the player whose turn it is and he is swapping out a piece
                switch (GameManager.turn)
                {
                    case PieceColor.white:
                        MoveToBoard(GameManager.blackPrison[row, column].piece);
                        GameManager.blackPrison[row, column].piece.SwapBackIn();
                        break;
                    case PieceColor.black:
                        MoveToBoard(GameManager.whitePrison[row, column].piece);
                        GameManager.whitePrison[row, column].piece.SwapBackIn();
                        break;
                }
            }
        }

        private void Restart_Clicked(object sender, RoutedEventArgs e)
        {
            // Reset all pieces to their original state
            foreach (Piece piece in pieces)
            {
                piece.Reset();
            }

            // Reset the turn
            GameManager.turn = PieceColor.white;
            TurnText.Text = "WHITE'S TURN";

            // Reset game state
            GameManager.swappingPiece = false;
            GameManager.gameOver = false;

            // Hide the end screen and show the turn text
            EndScreen.Visibility = Visibility.Collapsed;
            CheckmateText.Visibility = Visibility.Collapsed;
            WinText.Visibility = Visibility.Collapsed;
            RestartButton.Visibility = Visibility.Collapsed;
            TurnText.Visibility = Visibility.Visible;
        }

        /* ROW 1 */
        private void R1C1_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(0, 0);
        }

        private void R1C2_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(0, 1);
        }

        private void R1C3_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(0, 2);
        }

        private void R1C4_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(0, 3);
        }

        private void R1C5_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(0, 4);
        }

        private void R1C6_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(0, 5);
        }

        private void R1C7_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(0, 6);
        }

        private void R1C8_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(0, 7);
        }

        /* ROW 2 */
        private void R2C1_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(1, 0);
        }

        private void R2C2_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(1, 1);
        }

        private void R2C3_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(1, 2);
        }

        private void R2C4_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(1, 3);
        }

        private void R2C5_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(1, 4);
        }

        private void R2C6_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(1, 5);
        }

        private void R2C7_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(1, 6);
        }

        private void R2C8_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(1, 7);
        }

        /* ROW 3 */
        private void R3C1_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(2, 0);
        }

        private void R3C2_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(2, 1);
        }

        private void R3C3_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(2, 2);
        }

        private void R3C4_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(2, 3);
        }

        private void R3C5_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(2, 4);
        }

        private void R3C6_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(2, 5);
        }

        private void R3C7_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(2, 6);
        }

        private void R3C8_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(2, 7);
        }

        /* ROW 4 */
        private void R4C1_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(3, 0);
        }

        private void R4C2_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(3, 1);
        }

        private void R4C3_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(3, 2);
        }

        private void R4C4_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(3, 3);
        }

        private void R4C5_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(3, 4);
        }

        private void R4C6_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(3, 5);
        }

        private void R4C7_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(3, 6);
        }

        private void R4C8_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(3, 7);
        }

        /* ROW 5 */
        private void R5C1_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(4, 0);
        }

        private void R5C2_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(4, 1);
        }

        private void R5C3_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(4, 2);
        }

        private void R5C4_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(4, 3);
        }

        private void R5C5_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(4, 4);
        }

        private void R5C6_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(4, 5);
        }

        private void R5C7_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(4, 6);
        }

        private void R5C8_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(4, 7);
        }

        /* ROW 6 */
        private void R6C1_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(5, 0);
        }

        private void R6C2_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(5, 1);
        }

        private void R6C3_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(5, 2);
        }

        private void R6C4_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(5, 3);
        }

        private void R6C5_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(5, 4);
        }

        private void R6C6_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(5, 5);
        }

        private void R6C7_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(5, 6);
        }

        private void R6C8_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(5, 7);
        }

        /* ROW 7 */
        private void R7C1_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(6, 0);
        }

        private void R7C2_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(6, 1);
        }

        private void R7C3_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(6, 2);
        }

        private void R7C4_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(6, 3);
        }

        private void R7C5_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(6, 4);
        }

        private void R7C6_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(6, 5);
        }

        private void R7C7_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(6, 6);
        }

        private void R7C8_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(6, 7);
        }

        /* ROW 8 */
        private void R8C1_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(7, 0);
        }

        private void R8C2_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(7, 1);
        }

        private void R8C3_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(7, 2);
        }

        private void R8C4_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(7, 3);
        }

        private void R8C5_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(7, 4);
        }

        private void R8C6_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(7, 5);
        }

        private void R8C7_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(7, 6);
        }

        private void R8C8_Clicked(object sender, RoutedEventArgs e)
        {
            TileClicked(7, 7);
        }
        
        /* White Prison */
        private void WPrisonR1C1(object sender, RoutedEventArgs e)
        {
            PrisonClicked(0, 0, PieceColor.white);
        }

        private void WPrisonR2C1(object sender, RoutedEventArgs e)
        {
            PrisonClicked(1, 0, PieceColor.white);
        }

        private void WPrisonR3C1(object sender, RoutedEventArgs e)
        {
            PrisonClicked(2, 0, PieceColor.white);
        }

        private void WPrisonR4C1(object sender, RoutedEventArgs e)
        {
            PrisonClicked(3, 0, PieceColor.white);
        }

        private void WPrisonR1C2(object sender, RoutedEventArgs e)
        {
            PrisonClicked(0, 1, PieceColor.white);
        }

        private void WPrisonR2C2(object sender, RoutedEventArgs e)
        {
            PrisonClicked(1, 1, PieceColor.white);
        }

        private void WPrisonR3C2(object sender, RoutedEventArgs e)
        {
            PrisonClicked(2, 1, PieceColor.white);
        }

        private void WPrisonR4C2(object sender, RoutedEventArgs e)
        {
            PrisonClicked(3, 1, PieceColor.white);
        }

        /* Black Prison */
        private void BPrisonR1C1(object sender, RoutedEventArgs e)
        {
            PrisonClicked(0, 0, PieceColor.black);
        }

        private void BPrisonR2C1(object sender, RoutedEventArgs e)
        {
            PrisonClicked(1, 0, PieceColor.black);
        }

        private void BPrisonR3C1(object sender, RoutedEventArgs e)
        {
            PrisonClicked(2, 0, PieceColor.black);
        }

        private void BPrisonR4C1(object sender, RoutedEventArgs e)
        {
            PrisonClicked(3, 0, PieceColor.black);
        }

        private void BPrisonR1C2(object sender, RoutedEventArgs e)
        {
            PrisonClicked(0, 1, PieceColor.black);
        }

        private void BPrisonR2C2(object sender, RoutedEventArgs e)
        {
            PrisonClicked(1, 1, PieceColor.black);
        }

        private void BPrisonR3C2(object sender, RoutedEventArgs e)
        {
            PrisonClicked(2, 1, PieceColor.black);
        }

        private void BPrisonR4C2(object sender, RoutedEventArgs e)
        {
            PrisonClicked(3, 1, PieceColor.black);
        }
    }
}
