#region Using

using System;
using System.ComponentModel;
using System.Configuration;
using System.Text;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    [SettingsGroupName("MainWindow")]
    public sealed class MainWindowSettings : ApplicationSettingsBase
    {
        [UserScopedSetting]
        [DefaultSettingValue("-1.0")]
        public double WindowLeft
        {
            get { return (double) this["WindowLeft"]; }
            set { this["WindowLeft"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("-1.0")]
        public double WindowTop
        {
            get { return (double) this["WindowTop"]; }
            set { this["WindowTop"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("1024")]
        public double WindowWidth
        {
            get { return (double) this["WindowWidth"]; }
            set { this["WindowWidth"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("800")]
        public double WindowHeight
        {
            get { return (double) this["WindowHeight"]; }
            set { this["WindowHeight"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool WindowMaximized
        {
            get { return (bool) this["WindowMaximized"]; }
            set { this["WindowMaximized"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue(null)]
        public string ProjectFilePath
        {
            get { return (string) this["ProjectFilePath"]; }
            set { this["ProjectFilePath"] = value; }
        }
    }
}
