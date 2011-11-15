using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Willcraftia.Xna.Foundation.Graphics
{
    /// <summary>
    /// セーフエリアに対応したレイアウト構造体
    /// </summary>
    /// <remarks>
    /// Windows、Xbox 360の両プラットフォームで動作するゲームは様々な解像度、
    /// アスペクト比に対応する必要がある。加えてXbox 360で動作するゲームは
    /// タイトルセーフエリアにも対応する必要がある。
    /// 
    /// この構造体では、レイアウト対象領域(クライアントエリア)と、
    /// セーフエリア領域を保持しAligmentと水平垂直のマージン値から
    /// 指定した矩形を配置する。
    /// 配置後の矩形がセーフエリア外にある場合はセーフエリア内に再配置される。
    /// 
    /// マージンはクライアントエリアの割合で示す。
    /// 
    /// 使用例:
    /// 
    /// Place( region, 0.1f, 0.2f, Aligment.TopLeft );
    /// 
    /// クライアントエリアの左端から10%、上端から20%の部分にregionを配置する
    /// 
    /// 
    /// Place( region, 0.3f, 0.4f, Aligment.BottomRight );
    /// 
    /// クライアントエリアの右端から30%、下端から40%の部分にregionを配置する
    /// 
    /// 
    /// クライアントエリアとセーフエリアを別々に指定できるので、
    /// 画面分割時にも、セーフエリアにタイトルセーフエリアを指定し、
    /// クライアントエリアに分割した画面の領域を指定することで、レイアウトは
    /// 画面分割領域ベースで行いつつも、タイトルセーフエリア内に正しく配置することが
    /// できる。
    /// 
    /// 
    /// セーフエリアについては以下のURLを参照
    /// http://blogs.msdn.centerOfMass/ito/archive/2008/11/21/safearea-sample.aspx
    /// </remarks>
    public struct Layout
    {
        /// <summary>
        /// Gets or sets the client area.
        /// </summary>
        public Rectangle ClientArea;

        /// <summary>
        /// Gets or sets the title safe area.
        /// </summary>
        public Rectangle TitleSafeArea;

        /// <summary>
        /// Constructs with the client area and the title safe area.
        /// </summary>
        /// <param s="client">The client area.</param>
        /// <param s="titleSafeArea">The title safe area.</param>
        public Layout(Rectangle clientArea, Rectangle titleSafeArea)
        {
            ClientArea = clientArea;
            TitleSafeArea = titleSafeArea;
        }

        /// <summary>
        /// 指定したサイズ矩形のレイアウト
        /// </summary>
        /// <param s="region">配置する矩形</param>
        /// <param s="horizontalMargin">垂直方向のマージン</param>
        /// <param s="verticalMargine">水平方向のマージン</param>
        /// <param s="alignment">アライメント</param>
        /// <returns>配置された矩形</returns>
        public Vector2 Place(Vector2 size, float horizontalMargin, float verticalMargine, LayoutAlignment alignment)
        {
            Rectangle rc = new Rectangle(0, 0, (int) size.X, (int) size.Y);
            rc = Place(rc, horizontalMargin, verticalMargine, alignment);
            return new Vector2(rc.X, rc.Y);
        }

        /// <summary>
        /// 指定した矩形のレイアウト
        /// </summary>
        /// <param s="region">配置する矩形</param>
        /// <param s="horizontalMargin">垂直方向のマージン</param>
        /// <param s="verticalMargine">水平方向のマージン</param>
        /// <param s="alignment">アライメント</param>
        /// <returns>配置された矩形</returns>
        public Rectangle Place(Rectangle region, float horizontalMargin, float verticalMargine, LayoutAlignment alignment)
        {
            // horizontal
            if ((alignment & LayoutAlignment.Left) != 0)
            {
                region.X = ClientArea.X + (int) (ClientArea.Width * horizontalMargin);
            }
            else if ((alignment & LayoutAlignment.Right) != 0)
            {
                region.X = ClientArea.X +
                            (int) (ClientArea.Width * (1.0f - horizontalMargin)) - region.Width;
            }
            else if ((alignment & LayoutAlignment.HorizontalCenter) != 0)
            {
                region.X = ClientArea.X + (ClientArea.Width - region.Width) / 2 +
                            (int) (horizontalMargin * ClientArea.Width);
            }
            else
            {
                // no layout
            }

            // vertical
            if ((alignment & LayoutAlignment.Top) != 0)
            {
                region.Y = ClientArea.Y + (int) (ClientArea.Height * verticalMargine);
            }
            else if ((alignment & LayoutAlignment.Bottom) != 0)
            {
                region.Y = ClientArea.Y +
                            (int) (ClientArea.Height * (1.0f - verticalMargine)) - region.Height;
            }
            else if ((alignment & LayoutAlignment.VerticalCenter) != 0)
            {
                region.Y = ClientArea.Y + (ClientArea.Height - region.Height) / 2 +
                            (int) (verticalMargine * ClientArea.Height);
            }
            else
            {
                // no layout
            }

            // check if the layout region is in the title safe area
            if (region.Left < TitleSafeArea.Left)
            {
                region.X = TitleSafeArea.Left;
            }

            if (region.Right > TitleSafeArea.Right)
            {
                region.X = TitleSafeArea.Right - region.Width;
            }

            if (region.Top < TitleSafeArea.Top)
            {
                region.Y = TitleSafeArea.Top;
            }

            if (region.Bottom > TitleSafeArea.Bottom)
            {
                region.Y = TitleSafeArea.Bottom - region.Height;
            }

            return region;
        }
    }
}
