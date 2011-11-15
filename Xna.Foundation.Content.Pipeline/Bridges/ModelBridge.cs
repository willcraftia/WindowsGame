#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Bridges
{
    public sealed class ModelBridge
    {
        ExternalReference<NodeContent> model = new ExternalReference<NodeContent>();
        public ExternalReference<NodeContent> Model
        {
            get { return model; }
            set { model = value; }
        }

        string importer;
        public string Importer
        {
            get { return importer; }
            set { importer = value; }
        }

        string processor;
        public string Processor
        {
            get { return processor; }
            set { processor = value; }
        }

        Dictionary<string, object> parameters = new Dictionary<string, object>();
        public Dictionary<string, object> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
    }
}
