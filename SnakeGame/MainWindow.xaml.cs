using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnakeGame
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int GridCellSize = 20;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            DrawGameArea();
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
                    Width = GridCellSize,
                    Height = GridCellSize,
                    Fill = nextIsOdd ? Brushes.White : Brushes.LightBlue
                };

                GameArea.Children.Add(rect);
                Canvas.SetTop(rect, nextY);
                Canvas.SetLeft(rect, nextX);

                nextIsOdd = !nextIsOdd;
                nextX += GridCellSize;

                if (nextX >= GameArea.ActualWidth)
                {
                    nextX = 0;
                    nextY += GridCellSize;
                    rowCounter++;
                    nextIsOdd = rowCounter % 2 != 0;
                }

                if (nextY >= GameArea.ActualHeight)
                    doneDrawingBackground = true;
            }
        }
    }
}