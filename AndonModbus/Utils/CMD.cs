using System.Windows.Input;

namespace AndonModbus.Utils
{
    public static class CMD
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit",
                typeof(CMD), new InputGestureCollection()
                {
                    new KeyGesture(Key.O, ModifierKeys.Control)
                }
            );
    }
}
