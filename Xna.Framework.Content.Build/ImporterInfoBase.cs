#region Using

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public abstract class ImporterInfoBase : IImporterInfo
    {
        public string TypeName { get; private set; }
        public bool CacheImportedData { get; private set; }
        public string DefaultProcessor { get; private set; }
        public string DisplayName { get; private set; }

        List<string> fileExtensions = new List<string>();
        public IEnumerable<string> FileExtensions
        {
            get { return fileExtensions; }
        }

        protected ImporterInfoBase(string typeName, ContentImporterAttribute attribute)
        {
            TypeName = typeName;
            CacheImportedData = attribute.CacheImportedData;
            DefaultProcessor = attribute.DefaultProcessor;
            DisplayName = attribute.DisplayName;
            fileExtensions.AddRange(attribute.FileExtensions);
        }

        public abstract IContentImporter CreateInstance();
    }
}
