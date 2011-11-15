#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Bridges
{
    [ContentProcessor(DisplayName = "Willcraftia Model Bridge Processor")]
    public class ModelBridgeProcessor : ContentProcessor<ModelBridge, ModelContent>
    {
        public override ModelContent Process(ModelBridge input, ContentProcessorContext context)
        {
            var parameters = new OpaqueDataDictionary();
            foreach (var entry in input.Parameters)
            {
                parameters.Add(entry.Key, entry.Value);
            }

            return context.BuildAndLoadAsset<NodeContent, ModelContent>(
                input.Model, input.Processor, parameters, input.Importer);
        }
    }
}