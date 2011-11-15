#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

#endregion

namespace Willcraftia.Xna.Framework.Content.Pipeline.Xml
{
    /// <summary>
    /// XML コンテンツのヘルパ メソッドを提供するクラスです。
    /// </summary>
    public static class XmlContentHelper
    {
        /// <summary>
        /// 指定の XML ファイルからコンテンツをデシリアライズします。
        /// </summary>
        /// <typeparam name="T">コンテンツの型。</typeparam>
        /// <param name="filename">XML ファイル パス。</param>
        /// <returns>コンテンツ。</returns>
        public static T Deserialize<T>(string filename)
        {
            using (var reader = XmlReader.Create(filename))
            {
                return IntermediateSerializer.Deserialize<T>(reader, filename);
            }
        }

        /// <summary>
        /// 指定の XML ファイルへコンテンツをシリアライズします。
        /// </summary>
        /// <typeparam name="T">コンテンツの型。</typeparam>
        /// <param name="content">コンテンツ。</param>
        /// <param name="filename">XML ファイル パス。</param>
        public static void Serialize<T>(T content, string filename)
        {
            Serialize<T>(content, filename, true);
        }

        /// <summary>
        /// 指定の XML ファイルへコンテンツをシリアライズします。
        /// </summary>
        /// <typeparam name="T">コンテンツの型。</typeparam>
        /// <param name="content">コンテンツ。</param>
        /// <param name="filename">XML ファイル パス。</param>
        /// <param name="createDirectory">
        /// true (XML ファイル パスに含まれるディレクトリ階層を自動的に作成する場合)、false (それ以外の場合)。
        /// </param>
        public static void Serialize<T>(T content, string filename, bool createDirectory)
        {
            if (createDirectory)
            {
                var directory = Path.GetDirectoryName(filename);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            using (var writer = XmlWriter.Create(filename, settings))
            {
                IntermediateSerializer.Serialize<T>(writer, content, filename);
            }
        }
    }
}
