#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class PluginSettingsEdit
    {
        readonly PluginSettings pluginSettings;

        public List<string> AssemblyFiles { get; private set; }

        public PluginSettingsEdit(PluginSettings pluginSettings)
        {
            if (pluginSettings == null) throw new ArgumentNullException("pluginSettings");

            this.pluginSettings = pluginSettings;
            
            AssemblyFiles = new List<string>();

            // プラグイン未登録の場合には PluginSettings.AssemblyFiles が null である点に注意が必要です。
            if (pluginSettings.AssemblyFiles != null)
            {
                AssemblyFiles.AddRange(pluginSettings.AssemblyFiles);
            }
        }

        public void Save()
        {
            // リストの参照が共有されないように新しいリストへ要素をコピーしてから反映させます。
            List<string> assemblyFiles = new List<string>();
            foreach (var assemblyFile in AssemblyFiles)
            {
                assemblyFiles.Add(assemblyFile);
            }
            pluginSettings.AssemblyFiles = assemblyFiles;
            pluginSettings.Save();
        }
    }
}
