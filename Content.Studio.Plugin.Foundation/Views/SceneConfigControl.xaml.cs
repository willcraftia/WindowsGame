#region Using

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Willcraftia.Content.Studio.Plugin.Foundation.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Views
{
    /// <summary>
    /// SceneConfigControl.xaml の相互作用ロジック
    /// </summary>
    public partial class SceneConfigControl : UserControl
    {
        Vector3 defaultCameraPosition;

        public SceneConfigControl()
        {
            InitializeComponent();

            // コントロールに設定されたカメラ座標を初期値として記憶しておきます。
            defaultCameraPosition = SceneConfigFormControl.CameraPosition;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            var sceneConfigViewModel = DataContext as SceneConfigViewModel;
            if (sceneConfigViewModel != null)
            {
                SceneConfigFormControl.SceneConfigViewModel = DataContext as SceneConfigViewModel;
                SceneConfigFormControl.LoadContent();
            }
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (SceneConfigFormControl.SceneConfigViewModel != null)
            {
                SceneConfigFormControl.UnloadContent();
                SceneConfigFormControl.SceneConfigViewModel = null;
            }
        }

        void ResetCameraOrientationMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        void ResetCameraPositionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SceneConfigFormControl.CameraPosition = defaultCameraPosition;
        }
    }
}
