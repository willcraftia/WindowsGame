#region Using

using System.Collections.Generic;
using System.Configuration;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class PluginSettings : ApplicationSettingsBase
    {
        [UserScopedSetting]
        public List<string> AssemblyFiles
        {
            get { return (List<string>) this["AssemblyFiles"]; }
            set { this["AssemblyFiles"] = value; }
        }
    }
}
