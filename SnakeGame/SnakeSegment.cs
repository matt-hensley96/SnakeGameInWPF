using System.Windows;

namespace SnakeGame
{
    public class SnakeSegment
    {
        public UIElement UiElement { get; set; }
        public Point Position { get; set; }
        public bool IsHead { get; set; }
    }
}