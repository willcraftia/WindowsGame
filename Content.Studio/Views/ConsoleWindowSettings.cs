#region Using

using System;
using System.ComponentModel;
using System.Configuration;
using System.Text;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    [SettingsGroupName("ConsoleWindow")]
    public sealed class ConsoleWindowSettings : ApplicationSettingsBase
    {
        [UserScopedSetting]
        [DefaultSettingValue("30000")]
        public int MaxCharacters
        {
            get { return (int) this["MaxCharacters"]; }
            set { this["MaxCharacters"] = value; }
        }
    }
}
