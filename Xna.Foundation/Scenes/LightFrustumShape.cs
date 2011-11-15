
namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// Shadow Mapping で使用するライト カメラの視錐台の形状の列挙です。
    /// </summary>
    public enum LightFrustumShape
    {
        /// <summary>
        /// PSSM (Parallel Split Shadow Maps) です。
        /// </summary>
        Pssm,

        /// <summary>
        /// LSPSM (Light Space Perspective Shadow Maps) です。
        /// </summary>
        Lspsm,

        /// <summary>
        /// USM (Uniform Shadow Maps) です。
        /// </summary>
        Usm
    }
}
