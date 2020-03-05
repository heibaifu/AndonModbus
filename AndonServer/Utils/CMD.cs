using System.Windows.Input;

namespace AndonServer.Utils
{
    public static class CMD
    {
        public static readonly RoutedUICommand OPT = new RoutedUICommand("OPT", "OPT",
                typeof(CMD), new InputGestureCollection()
                {
                    new KeyGesture(Key.O, ModifierKeys.Control)
                }
            );
    }
}
