using System.Collections.ObjectModel;
using System.IO.Enumeration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    public class AllowableSizes : ObservableCollection<int>
    {
        public AllowableSizes()
        {
            for (int i = 3; i < 21; i++)
            {
                this.Add(i);
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlaySpace playSpace;
        GameState gameState;
        Direction direction;
        Direction lastDirection;
        DispatcherTimer dispatcherTimer;
        AllowableSizes allowableSizes;

        Rectangle[,] rectangle;

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += OnKeyDownHandler;
            allowableSizes = new AllowableSizes();
        }
        private void GameLoop(object sender, EventArgs e)
        {
            //GetDirection();
            Direction thisDirection = direction;
            Gobble.Visibility = Visibility.Hidden;

            gameState = playSpace.update(thisDirection);
            Draw();

            if (gameState == GameState.DEAD)
            {
                dispatcherTimer.Stop();
                GameOver.Visibility = Visibility.Visible;

                NewGame.IsEnabled = WIDTH.IsEnabled = HEIGHT.IsEnabled = true;
            }
            if (gameState == GameState.WIN)
            {
                dispatcherTimer.Stop();
                GameWin.Visibility = Visibility.Visible;

                NewGame.IsEnabled = WIDTH.IsEnabled = HEIGHT.IsEnabled = true;
            }
            if(gameState == GameState.GOBBLE) Gobble.Visibility = Visibility.Visible;
            lastDirection = thisDirection;
        }

        private void Draw()
        {
            CellType[,] gameStates = playSpace.GetGameState();
            int[] head = playSpace.GetHeadOfSnake();

            for (int j = 0; j < gameStates.GetLength(1); j++)
            {
                for (int i = 0; i < gameStates.GetLength(0); i++)
                {
                    switch (gameStates[i,j])
                    {
                        case CellType.WALL:
                            rectangle[i, j].Fill = new SolidColorBrush(Colors.Beige);
                            rectangle[i, j].Stroke = new SolidColorBrush(Colors.Black);
                            break;
                        case CellType.EMPTY:
                            rectangle[i, j].Fill = new SolidColorBrush(Colors.White);
                            rectangle[i, j].Stroke = new SolidColorBrush(Colors.White);
                            break;
                        case CellType.FOOD:
                            rectangle[i, j].Fill = new SolidColorBrush(Colors.Red);
                            rectangle[i, j].RadiusX = rectangle[i, j].RadiusY = 15;
                            rectangle[i, j].Stroke = new SolidColorBrush(Colors.Black);
                            break;
                        case CellType.SNAKEPART:
                            if (head[0] == i && head[1] == j) rectangle[i, j].Fill = new SolidColorBrush(Colors.DarkOrange);
                            else rectangle[i, j].Fill = new SolidColorBrush(Colors.Orange);
                            rectangle[i, j].RadiusX = rectangle[i, j].RadiusY = 12;
                            rectangle[i, j].Stroke = new SolidColorBrush(Colors.Brown);
                            break;
                        default:
                            break;
                    }
                }
            }
            ChangeDirection(direction);
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.A:
                    ChangeDirection(Direction.WEST);
                    break;

                case Key.Up:
                case Key.W:
                    ChangeDirection(Direction.NORTH);
                    break;

                case Key.Right:
                case Key.D:
                    ChangeDirection(Direction.EAST);
                    break;

                case Key.Down:
                case Key.S:
                    ChangeDirection(Direction.SOUTH);
                    break;

                default:
                    break;
            }
        }
        private void ChangeDirection(Direction thisDirection)
        {
            switch (thisDirection)
            {
                case Direction.WEST:
                    if (lastDirection != Direction.EAST)
                    {
                        direction = Direction.WEST;
                        ControlVisible(WEST);
                    }
                    break;

                case Direction.NORTH:
                    if (lastDirection != Direction.SOUTH)
                    {
                        direction = Direction.NORTH;
                        ControlVisible(NORTH);
                    }
                    break;

                case Direction.EAST:
                    if (lastDirection != Direction.WEST)
                    {
                        direction = Direction.EAST;
                        ControlVisible(EAST);
                    }
                    break;

                case Direction.SOUTH:
                    if (lastDirection != Direction.NORTH)
                    {
                        direction = Direction.SOUTH;
                        ControlVisible(SOUTH);
                    }
                    break;

                default:
                    break;
         
            }
        }
        private void ControlVisible(Image image)
        {
            if (image != NORTH) NORTH.Visibility = Visibility.Hidden;
            else NORTH.Visibility = Visibility.Visible;
            if (image != EAST) EAST.Visibility = Visibility.Hidden;
            else EAST.Visibility = Visibility.Visible;  
            if (image != SOUTH) SOUTH.Visibility = Visibility.Hidden;
            else SOUTH.Visibility = Visibility.Visible;
            if (image != WEST) WEST.Visibility = Visibility.Hidden;
            else WEST.Visibility = Visibility.Visible;
        }
        private void GetDirection()
        {
            int[] headOfsnake = playSpace.GetHeadOfSnake();
            Point mousePoint = Mouse.GetPosition(this);
            Point snakePoint = new Point(this.ActualWidth/2, this.ActualHeight/2);
            Vector vector = new Vector(mousePoint.X-snakePoint.X, mousePoint.Y-snakePoint.Y);
            if (Math.Abs(vector.X) > Math.Abs(vector.Y)) 
            {
                if (vector.X < 0) direction = Direction.WEST;
                else direction = Direction.EAST;
            }
            else
            {
                if (vector.Y < 0) direction = Direction.NORTH;
                else direction = Direction.SOUTH;
            }

        }
        private void NewGame_Click(object sender, EventArgs e)
        {
            NewGame.IsEnabled = WIDTH.IsEnabled = HEIGHT.IsEnabled = false;
            GameOver.Visibility = Visibility.Hidden;
            GameWin.Visibility = Visibility.Hidden;
            gameState = GameState.ALIVE;
            direction = Direction.NORTH;
            SnakeGrid.MaxHeight = double.PositiveInfinity;
            SnakeGrid.MaxWidth = double.PositiveInfinity;
            SnakeGrid.Background = new SolidColorBrush(Colors.White);

            playSpace = new PlaySpace((int)WIDTH.SelectedItem, (int)HEIGHT.SelectedItem);

            SnakeGrid.RowDefinitions.Clear();
            SnakeGrid.ColumnDefinitions.Clear();

            CellType[,] gameStates = playSpace.GetGameState();

            RowDefinition[] rowDefinitions = new RowDefinition[gameStates.GetLength(1)];
            for (int i = 0; i < rowDefinitions.Length; i++)
            {
                rowDefinitions[i] = new RowDefinition();
                SnakeGrid.RowDefinitions.Add(rowDefinitions[i]);
            }

            ColumnDefinition[] columnDefinitions = new ColumnDefinition[gameStates.GetLength(0)];
            for (int i = 0; i < columnDefinitions.Length; i++)
            {
                columnDefinitions[i] = new ColumnDefinition();
                SnakeGrid.ColumnDefinitions.Add(columnDefinitions[i]);
            }

            if (WIDTH.SelectedIndex > HEIGHT.SelectedIndex) SnakeGrid.MaxHeight = SnakeGrid.ActualHeight*(int)HEIGHT.SelectedItem/(int)WIDTH.SelectedItem;
            else SnakeGrid.MaxWidth = SnakeGrid.ActualWidth*(int)WIDTH.SelectedItem/(int)HEIGHT.SelectedItem;

            rectangle = new Rectangle[gameStates.GetLength(0), gameStates.GetLength(1)];
            for (int j = 0; j < gameStates.GetLength(1); j++)
            {
                for (int i = 0; i < gameStates.GetLength(0); i++)
                {
                    rectangle[i, j] = new Rectangle();
                    switch (gameStates[i, j])
                    {
                        case CellType.WALL:
                            rectangle[i, j].Fill = new SolidColorBrush(Colors.Beige);
                            break;
                        case CellType.EMPTY:
                            rectangle[i, j].Fill = new SolidColorBrush(Colors.White);
                            break;
                        case CellType.FOOD:
                            rectangle[i, j].Fill = new SolidColorBrush(Colors.Blue);
                            break;
                        case CellType.SNAKEPART:
                            rectangle[i, j].Fill = new SolidColorBrush(Colors.Orange);
                            break;
                        default:
                            break;
                    }
                    Grid.SetRow(rectangle[i, j], j);
                    Grid.SetColumn(rectangle[i, j], i);
                    SnakeGrid.Children.Add(rectangle[i, j]);
                }
            }

            Draw();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(GameLoop);
            dispatcherTimer.Interval = new TimeSpan(5000000);
            dispatcherTimer.Start();
        }
    }
}