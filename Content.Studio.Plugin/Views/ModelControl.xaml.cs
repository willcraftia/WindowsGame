#region Using

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Views
{
    /// <summary>
    /// ModelControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ModelControl : UserControl
    {
        Vector3 defaultPovPosition;
        Vector3 defaultModelOrientation;

        public ModelControl()
        {
            InitializeComponent();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            ModelFormControl.RuntimeContent = DataContext as RuntimeContentViewModel;
            ModelFormControl.LoadContent();

            // コントロールに設定されたカメラ座標とモデル姿勢行列を初期値として記憶しておきます。
            defaultPovPosition = ModelFormControl.PovPosition;
            defaultModelOrientation = ModelFormControl.ModelOrientation;
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ModelFormControl.UnloadContent();
            ModelFormControl.RuntimeContent = null;
        }

        void ResetModelOrientationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ModelFormControl.ModelOrientation = defaultModelOrientation;
        }

        void ResetCameraPositionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ModelFormControl.PovPosition = defaultPovPosition;
        }
    }
}
