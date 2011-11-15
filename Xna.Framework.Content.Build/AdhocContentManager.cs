#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    /// <summary>
    /// キャッシュすることなく常にコンテンツのロードを試行する ContentManager です。
    /// </summary>
    /// <remarks>
    /// XNA 標準 ContentManager は、一度ロードしたアセットをコンテンツします。
    /// しかし、コンテンツを編集するアプリケーションでは編集に応じて再ビルドを行う必要があり、
    /// XNA 標準 ContentManager ではキャッシュされたコンテンツを取得してしまう状況が問題となります。
    /// そこで、キャッシュすることなく常に再ロードを試行する AdhocContentManager を用います。
    /// </remarks>
    public sealed class AdhocContentManager : ContentManager
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider。</param>
        /// <param name="rootDirectory">
        /// ロードするコンテンツのルート ディレクトリ。
        /// XNA 標準 ContentManager とは異なり、絶対パスを指定する必要があります。
        /// </param>
        public AdhocContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
        }

        /// <summary>
        /// 指定されたアセット名に対応するコンテンツをロードします。
        /// </summary>
        /// <typeparam name="T">コンテンツ型。</typeparam>
        /// <param name="assetName">アセット名。</param>
        /// <returns>ロードされたコンテンツ。</returns>
        public override T Load<T>(string assetName)
        {
            T result = ReadAsset<T>(assetName, null);

            Tracer.TraceSource.TraceInformation("Loaded the content of '{0}'.", assetName);

            return result;
        }
    }
}
