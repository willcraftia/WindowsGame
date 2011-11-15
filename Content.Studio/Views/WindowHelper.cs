#region Using

using System.Windows;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public static class WindowHelper
    {
        public static void PositionToCenter(Window targetWindow, DependencyObject dependencyObject)
        {
            var baseWindow = Window.GetWindow(dependencyObject);

            // ウィンドウの座標を中心に移動させます。
            // ウィンドウのサイズは動的に決定される場合があるため、
            // 描画準備の完了で中心を計算して設定するようにします。
            targetWindow.Left = baseWindow.Left + (baseWindow.Width - targetWindow.Width) * 0.5f;
            targetWindow.Top = baseWindow.Top + (baseWindow.Height - targetWindow.Height) * 0.5f;
        }
    }
}
