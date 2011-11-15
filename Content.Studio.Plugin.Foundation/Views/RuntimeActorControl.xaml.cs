#region Using

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Views
{
    using XnaMatrix = Microsoft.Xna.Framework.Matrix;

    /// <summary>
    /// ActorControl.xaml の相互作用ロジック
    /// </summary>
    public partial class RuntimeActorControl : UserControl
    {
        static XnaMatrix defaultActorOrientation = XnaMatrix.CreateRotationY(-MathHelper.PiOver4);

        Vector3 defaultCameraPosition;

        public RuntimeActorControl()
        {
            InitializeComponent();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            ActorFormControl.RuntimeContent = DataContext as RuntimeContentViewModel;
            ActorFormControl.LoadContent();
            ActorFormControl.ResetActorOrientation(defaultActorOrientation);

            // コントロールに設定されたカメラ座標とモデル姿勢行列を初期値として記憶しておきます。
            defaultCameraPosition = ActorFormControl.CameraPosition;

            // 光源をデフォルトとは違う位置に設定します (デフォルトは方向が Vector3.Down の光源)。
            var direction = new Vector3(1, -1, -1);
            direction.Normalize();
            ActorFormControl.SceneSettings.DirectionalLight0.Direction = direction;
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ActorFormControl.UnloadContent();
            ActorFormControl.RuntimeContent = null;
        }

        void ResetModelOrientationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ActorFormControl.ResetActorOrientation(defaultActorOrientation);
        }

        void ResetCameraPositionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ActorFormControl.CameraPosition = defaultCameraPosition;
        }
    }
}
