﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;

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
        private const int _maxHighScoreListEntryCount = 5;
        private readonly SolidColorBrush _foodBrush = Brushes.Red;
        private readonly Random _random = new Random();
        private readonly SolidColorBrush _snakeBodyBrush = Brushes.Green;
        private readonly SolidColorBrush _snakeHeadBrush = Brushes.YellowGreen;
        private readonly List<SnakeSegment> _snakeSegments = new List<SnakeSegment>();
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private int _currentScore;
        private int _snakeLength;

        private SnakeDirection _snakeDirection = SnakeDirection.Right;
        private UIElement _snakeFood;

        public MainWindow()
        {
            InitializeComponent();
            _timer.Tick += Timer_Tick;
            LoadHighScoreList();
        }

        public ObservableCollection<HighScore> HighScores { get; set; } = new ObservableCollection<HighScore>();

        private void Timer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
        }

        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            DrawGameArea();
            PlayOpenAppSound();
        }

        private static void PlayGameOverSound()
        {
            var gameOverMediaPlayer = new MediaPlayer();
            gameOverMediaPlayer.Open(new Uri("../../../Sounds/GameOver.mp3", UriKind.Relative));
            gameOverMediaPlayer.Play();
        }

        private static void PlayCrashSelfSound()
        {
            var crashSelfMediaPlayer = new MediaPlayer();
            crashSelfMediaPlayer.Open(new Uri("../../../Sounds/CrashSelf.mp3", UriKind.Relative));
            crashSelfMediaPlayer.Play();
        }

        private static void PlayCrashWallSound()
        {
            var crashWallMediaPlayer = new MediaPlayer();
            crashWallMediaPlayer.Open(new Uri("../../../Sounds/CrashWall.mp3", UriKind.Relative));
            crashWallMediaPlayer.Play();
        }

        private static void PlayEatSound()
        {
            var eatMediaPlayer = new MediaPlayer();
            eatMediaPlayer.Open(new Uri("../../../Sounds/Eat.mp3", UriKind.Relative));
            eatMediaPlayer.Play();
        }

        private static void PlayStartGameSound()
        {
            var startGameMediaPlayer = new MediaPlayer();
            startGameMediaPlayer.Open(new Uri("../../../Sounds/StartGame.mp3", UriKind.Relative));
            startGameMediaPlayer.Play();
        }

        private static void PlayOpenAppSound()
        {
            var openAppMediaPlayer = new MediaPlayer();
            openAppMediaPlayer.Open(new Uri("../../../Sounds/OpenApp.mp3", UriKind.Relative));
            openAppMediaPlayer.Play();
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

        private void LoadHighScoreList()
        {
            if (!File.Exists("HighScoreList.xml"))
                return;

            var serializer = new XmlSerializer(typeof(List<HighScore>));

            using (Stream reader = new FileStream("HighScoreList.xml", FileMode.Open))
            {
                var tempList = (List<HighScore>)serializer.Deserialize(reader);
                HighScores.Clear();

                foreach (HighScore item in tempList.OrderByDescending(x => x.Score))
                    HighScores.Add(item);
            }
        }

        private void SaveHighScoreList()
        {
            var serializer = new XmlSerializer(typeof(ObservableCollection<HighScore>));

            using (Stream writer = new FileStream("HighScoreList.xml", FileMode.Create))
            {
                serializer.Serialize(writer, HighScores);
            }
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
            BorderWelcomeMessage.Visibility = Visibility.Collapsed;
            BorderHighScoreList.Visibility = Visibility.Collapsed;
            BorderEndOfGame.Visibility = Visibility.Collapsed;

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
            PlayStartGameSound();

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
                EatFood();

            if (snakeHead.Position.Y < 0 ||
                snakeHead.Position.Y >= GameArea.ActualHeight ||
                snakeHead.Position.X < 0 ||
                snakeHead.Position.X >= GameArea.ActualWidth)
            {
                PlayCrashWallSound();
                EndGame();
            }

            foreach (SnakeSegment snakeSegment in _snakeSegments.Take(_snakeSegments.Count - 1))
                if (snakeHead.Position.X == snakeSegment.Position.X &&
                    snakeHead.Position.Y == snakeSegment.Position.Y)
                {
                    PlayCrashSelfSound();
                    EndGame();
                }
        }

        private void EatFood()
        {
            _snakeLength++;
            _currentScore++;

            int timerInterval = Math.Max(_snakeSpeedThreshold, (int)_timer.Interval.TotalMilliseconds - _currentScore * 2);
            _timer.Interval = TimeSpan.FromMilliseconds(timerInterval);

            GameArea.Children.Remove(_snakeFood);
            PlayEatSound();

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
            PlayGameOverSound();

            var isNewHighScore = false;

            if (_currentScore > 0)
            {
                int lowestHighScore = HighScores.Count > 0 ? HighScores.Min(x => x.Score) : 0;
                if (_currentScore > lowestHighScore || HighScores.Count < _maxHighScoreListEntryCount)
                {
                    BorderNewHighScore.Visibility = Visibility.Visible;
                    TextBoxPlayerName.Focus();
                    isNewHighScore = true;
                }
            }

            if (!isNewHighScore)
            {
                TextBlockFinalScore.Text = _currentScore.ToString();
                BorderEndOfGame.Visibility = Visibility.Visible;
            }

            _timer.IsEnabled = false;
        }

        private void BtnShowHighScoreList_Click(object sender, RoutedEventArgs e)
        {
            BorderWelcomeMessage.Visibility = Visibility.Collapsed;
            BorderHighScoreList.Visibility = Visibility.Visible;
        }

        private void BtnAddToHighScoreList_Click(object sender, RoutedEventArgs e)
        {
            var newIndex = 0;

            if (HighScores.Count > 0 && _currentScore < HighScores.Max(hs => hs.Score))
            {
                HighScore scoreAboveCurrentScore = HighScores
                    .OrderBy(hs => hs.Score)
                    .First(hs => hs.Score >= _currentScore);

                if (scoreAboveCurrentScore != null)
                    newIndex = HighScores.IndexOf(scoreAboveCurrentScore) + 1;
            }

            HighScores.Insert(newIndex, new HighScore
            {
                PlayerName = TextBoxPlayerName.Text,
                Score = _currentScore
            });

            while (HighScores.Count > _maxHighScoreListEntryCount)
                HighScores.RemoveAt(_maxHighScoreListEntryCount);

            SaveHighScoreList();

            BorderNewHighScore.Visibility = Visibility.Collapsed;
            BorderHighScoreList.Visibility = Visibility.Visible;
        }
    }
}