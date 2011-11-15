#region Using

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Willcraftia.Xna.Foundation.Cube.Scenes;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Cube.Views
{
    /// <summary>
    /// CubeTerrainControl.xaml の相互作用ロジック
    /// </summary>
    public partial class RuntimeCubeTerrainControl : UserControl
    {
        Vector3 defaultPovPosition;
        Vector3 defaultModelOrientation;

        public RuntimeCubeTerrainControl()
        {
            InitializeComponent();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            CubeTerrainFormControl.RuntimeContent = DataContext as RuntimeContentViewModel;
            CubeTerrainFormControl.LoadContent();

            // コントロールに設定されたカメラ座標とモデル姿勢行列を初期値として記憶しておきます。
            defaultPovPosition = CubeTerrainFormControl.PovPosition;
            defaultModelOrientation = CubeTerrainFormControl.ModelOrientation;
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CubeTerrainFormControl.UnloadContent();
            CubeTerrainFormControl.RuntimeContent = null;
        }

        void ResetModelOrientationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CubeTerrainFormControl.ModelOrientation = defaultModelOrientation;
        }

        void ResetCameraPositionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CubeTerrainFormControl.PovPosition = defaultPovPosition;
        }
    }
}
