#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class RuntimeContentTypeManager
    {
        //
        // NOTE:
        //
        // このクラスは、未ビルド状態で実行時コンテンツ型を解決するために用意しています。
        //

        static RuntimeContentTypeManager instance = new RuntimeContentTypeManager();
        public static RuntimeContentTypeManager Instance
        {
            get { return instance; }
        }

        static Dictionary<Type, Type> standardTypeMapping = new Dictionary<Type, Type>();
        
        static RuntimeContentTypeManager()
        {
            standardTypeMapping.Add(typeof(EffectContent), typeof(Effect));
            standardTypeMapping.Add(typeof(MaterialContent), typeof(Effect));
            standardTypeMapping.Add(typeof(EffectMaterialContent), typeof(Effect));
            standardTypeMapping.Add(typeof(ModelContent), typeof(Model));
            standardTypeMapping.Add(typeof(TextureContent), typeof(Texture));
            standardTypeMapping.Add(typeof(Texture2DContent), typeof(Texture2D));
            standardTypeMapping.Add(typeof(Texture3DContent), typeof(Texture3D));
            standardTypeMapping.Add(typeof(SpriteFontContent), typeof(SpriteFont));
        }

        RuntimeContentTypeManager() { }

        public Type ResolveRuntimeContentType(Asset asset)
        {
            if (asset == null) throw new ArgumentNullException("asset");

            // プロセッサを使用しないアセットの実行時コンテンツ型はビルド前コンテンツ型です。
            if (asset.Processor == null)
            {
                // TODO: Importer から解決できるようにしないとダメ。
                return ContentTypeResolverManager.Instance.ResolveContentType(asset.File);
            }

            var processorInfo = asset.Project.ProcessorInfoRegistry.GetProcessorInfo(asset.Processor);
            if (processorInfo == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0, "ContentProcessor '{0}' not found.", asset.Processor);
                return null;
            }

            var processor = processorInfo.CreateInstance();
            var outputType = processor.OutputType;

            // XNA 標準ビルド後コンテンツ型から実行時コンテンツ型を解決します。
            var runtimeType = ResolveStandardRuntimeContentType(outputType);
            if (runtimeType != null)
            {
                return runtimeType;
            }

            // ContentSerializerRuntimeTypeAttribute の指定から実行時コンテンツ型を解決します。
            runtimeType = ResolveCustomRuntimeContentType(outputType);
            if (runtimeType != null)
            {
                return runtimeType;
            }

            // それ以外の場合はビルド後コンテンツ型が実行時コンテンツ型です。
            return outputType;
        }

        /// <summary>
        /// 指定したビルド後コンテンツ型が XNA 標準であることを仮定して、
        /// 対応する実行時コンテンツ型を取得します。
        /// </summary>
        /// <param name="outputType">ビルド後コンテンツ型。</param>
        /// <returns>
        /// 実行時コンテンツ型 (XNA 標準のビルド後コンテンツ型である場合)、null (それ以外の場合)。
        /// </returns>
        Type ResolveStandardRuntimeContentType(Type outputType)
        {
            Type runtimeType = null;
            standardTypeMapping.TryGetValue(outputType, out runtimeType);
            return runtimeType;
        }

        /// <summary>
        /// 指定したビルド後コンテンツ型に ContentSerializerRuntimeTypeAttribute が指定されていることを仮定して、
        /// 対応する実行時コンテンツ型を取得します。
        /// </summary>
        /// <param name="outputType">ビルド後コンテンツ型。</param>
        /// <returns>
        /// 実行時コンテンツ型 (ContentSerializerRuntimeTypeAttribute の指定が見つかった場合)、null (それ以外の場合)。
        /// </returns>
        Type ResolveCustomRuntimeContentType(Type outputType)
        {
            var attribute = Attribute.GetCustomAttribute(outputType, typeof(ContentSerializerRuntimeTypeAttribute));
            if (attribute == null)
            {
                return null;
            }

            var runtimeTypeString = (attribute as ContentSerializerRuntimeTypeAttribute).RuntimeType;
            return Type.GetType(runtimeTypeString);
        }
    }
}
