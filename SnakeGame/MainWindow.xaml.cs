using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum SnakeDirection
        {
            Left,
            Right,
            Up,
            Down
        }

        private const int _gridCellSize = 20;
        private const int _snakeStartLength = 3;
        private const int _snakeStartSpeed = 400;
        private const int _snakeSpeedThreshold = 100;
        private readonly SolidColorBrush _foodBrush = Brushes.Red;
        private readonly Random _random = new Random();
        private readonly SolidColorBrush _snakeBodyBrush = Brushes.Green;
        private readonly SolidColorBrush _snakeHeadBrush = Brushes.YellowGreen;
        private readonly List<SnakeSegment> _snakeSegments = new List<SnakeSegment>();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private int _currentScore;

        private SnakeDirection _snakeDirection = SnakeDirection.Right;
        private UIElement _snakeFood;
        private int _snakeLength;

        public MainWindow()
        {
            InitializeComponent();
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
        }

        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            DrawGameArea();
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            SnakeDirection originalSnakeDirection = _snakeDirection;

            switch (e.Key)
            {
                case Key.Up:
                    if (_snakeDirection != SnakeDirection.Down)
                        _snakeDirection = SnakeDirection.Up;
                    break;

                case Key.Down:
                    if (_snakeDirection != SnakeDirection.Up)
                        _snakeDirection = SnakeDirection.Down;
                    break;

                case Key.Left:
                    if (_snakeDirection != SnakeDirection.Right)
                        _snakeDirection = SnakeDirection.Left;
                    break;

                case Key.Right:
                    if (_snakeDirection != SnakeDirection.Left)
                        _snakeDirection = SnakeDirection.Right;
                    break;

                case Key.Space:
                    StartNewGame();
                    break;
            }

            if (_snakeDirection != originalSnakeDirection)
                MoveSnake();
        }

        private void DrawGameArea()
        {
            var doneDrawingBackground = false;
            var nextX = 0;
            var nextY = 0;
            var rowCounter = 0;
            var nextIsOdd = false;

            while (doneDrawingBackground == false)
            {
                var rect = new Rectangle
                {
                    Width = _gridCellSize,
                    Height = _gridCellSize,
                    Fill = nextIsOdd ? new SolidColorBrush(Color.FromRgb(63, 63, 63)) : new SolidColorBrush(Color.FromRgb(79, 79, 79))
                };

                GameArea.Children.Add(rect);
                Canvas.SetTop(rect, nextY);
                Canvas.SetLeft(rect, nextX);

                nextIsOdd = !nextIsOdd;
                nextX += _gridCellSize;

                if (nextX >= GameArea.ActualWidth)
                {
                    nextX = 0;
                    nextY += _gridCellSize;
                    rowCounter++;
                    nextIsOdd = rowCounter % 2 != 0;
                }

                if (nextY >= GameArea.ActualHeight)
                    doneDrawingBackground = true;
            }
        }

        private void StartNewGame()
        {
            foreach (SnakeSegment snakeSegment in _snakeSegments
                .Where(snakeSegment => snakeSegment.UiElement != null))
                GameArea.Children.Remove(snakeSegment.UiElement);

            _snakeSegments.Clear();

            if (_snakeFood != null)
                GameArea.Children.Remove(_snakeFood);

            _currentScore = 0;
            _snakeLength = _snakeStartLength;
            _snakeDirection = SnakeDirection.Right;

            _snakeSegments.Add(new SnakeSegment
                { Position = new Point(_gridCellSize * 5, _gridCellSize * 5) });

            _timer.Interval = TimeSpan.FromMilliseconds(_snakeStartSpeed);

            DrawSnake();
            DrawSnakeFood();

            UpdateGameStatus();
            _timer.IsEnabled = true;
        }

        private void DrawSnake()
        {
            foreach (SnakeSegment snakeSegment in _snakeSegments.Where(snakeSegment => snakeSegment.UiElement == null))
            {
                snakeSegment.UiElement = new Rectangle
                {
                    Width = _gridCellSize,
                    Height = _gridCellSize,
                    Fill = snakeSegment.IsHead ? _snakeHeadBrush : _snakeBodyBrush
                };

                GameArea.Children.Add(snakeSegment.UiElement);
                Canvas.SetTop(snakeSegment.UiElement, snakeSegment.Position.Y);
                Canvas.SetLeft(snakeSegment.UiElement, snakeSegment.Position.X);
            }
        }

        private void DrawSnakeFood()
        {
            Point foodPosition = GetNextFoodPosition();

            _snakeFood = new Ellipse
            {
                Width = _gridCellSize,
                Height = _gridCellSize,
                Fill = _foodBrush
            };

            GameArea.Children.Add(_snakeFood);
            Canvas.SetTop(_snakeFood, foodPosition.Y);
            Canvas.SetLeft(_snakeFood, foodPosition.X);
        }

        private Point GetNextFoodPosition()
        {
            while (true)
            {
                int maxX = (int)GameArea.ActualWidth / _gridCellSize;
                int maxY = (int)GameArea.ActualHeight / _gridCellSize;
                int foodX = _random.Next(0, maxX) * _gridCellSize;
                int foodY = _random.Next(0, maxY) * _gridCellSize;

                if (_snakeSegments.Any(snakeSegment => snakeSegment.Position.X == foodX && snakeSegment.Position.Y == foodY))
                    continue;

                return new Point(foodX, foodY);
            }
        }

        private void MoveSnake()
        {
            while (_snakeSegments.Count >= _snakeLength)
            {
                GameArea.Children.Remove(_snakeSegments[0].UiElement);
                _snakeSegments.RemoveAt(0);
            }

            foreach (SnakeSegment snakeSegment in _snakeSegments)
            {
                ((Rectangle)snakeSegment.UiElement).Fill = _snakeBodyBrush;
                snakeSegment.IsHead = false;
            }

            SnakeSegment snakeHead = _snakeSegments[^1];
            double nextX = snakeHead.Position.X;
            double nextY = snakeHead.Position.Y;

            switch (_snakeDirection)
            {
                case SnakeDirection.Left:
                    nextX -= _gridCellSize;
                    break;
                case SnakeDirection.Right:
                    nextX += _gridCellSize;
                    break;
                case SnakeDirection.Up:
                    nextY -= _gridCellSize;
                    break;
                case SnakeDirection.Down:
                    nextY += _gridCellSize;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _snakeSegments.Add(new SnakeSegment
            {
                Position = new Point(nextX, nextY),
                IsHead = true
            });

            DrawSnake();

            DoCollisionCheck();
        }

        private void DoCollisionCheck()
        {
            SnakeSegment snakeHead = _snakeSegments[^1];

            if (snakeHead.Position.X == Canvas.GetLeft(_snakeFood) &&
                snakeHead.Position.Y == Canvas.GetTop(_snakeFood))
                EatSnakeFood();

            if (snakeHead.Position.Y < 0 ||
                snakeHead.Position.Y >= GameArea.ActualHeight ||
                snakeHead.Position.X < 0 ||
                snakeHead.Position.X >= GameArea.ActualWidth)
                EndGame();

            foreach (SnakeSegment snakeSegment in _snakeSegments.Take(_snakeSegments.Count - 1))
                if (snakeHead.Position.X == snakeSegment.Position.X &&
                    snakeHead.Position.Y == snakeSegment.Position.Y)
                    EndGame();
        }

        private void EatSnakeFood()
        {
            _snakeLength++;
            _currentScore++;

            int timerInterval = Math.Max(_snakeSpeedThreshold, (int)_timer.Interval.TotalMilliseconds - _currentScore * 2);
            _timer.Interval = TimeSpan.FromMilliseconds(timerInterval);

            GameArea.Children.Remove(_snakeFood);
            DrawSnakeFood();

            UpdateGameStatus();
        }

        private void UpdateGameStatus()
        {
            TextBlockScoreValue.Text = _currentScore.ToString();
            TextBlockSpeedValue.Text = _timer.Interval.TotalMilliseconds.ToString();
        }

        private void EndGame()
        {
            _timer.IsEnabled = false;

            MessageBox.Show("Uh oh! You died! \n\n Press space bar to try again!", "SnakeWPF");
        }
    }
}