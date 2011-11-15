#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public struct PostProcessSettings
    {
        #region Fields

        public bool DofEnabled;
        public DofSettings DofSettings;
        public bool SsaoEnabled;
        public SsaoSettings SsaoSettings;
        public bool GodRayEnabled;
        public GodRaySettings GodRaySettings;
        public bool MonochromeEnabled;
        public MonochromeSettings MonochromeSettings;
        public bool EdgeDetectionEnabled;
        public EdgeDetectionSettings EdgeDetectionSettings;
        public bool BloomEnabled;
        public BloomSettings BloomSettings;
        public bool ColorOverlapEnabled;
        public ColorOverlapSettings ColorOverlapSettings;

        #endregion

        #region Predefined

        public static PostProcessSettings Default
        {
            get
            {
                return new PostProcessSettings()
                {
                    DofEnabled = false,
                    DofSettings = DofSettings.Default,

                    SsaoEnabled = false,
                    SsaoSettings = SsaoSettings.Default,

                    GodRayEnabled = false,
                    GodRaySettings = GodRaySettings.Default,

                    MonochromeEnabled = false,
                    MonochromeSettings = MonochromeSettings.Default,

                    EdgeDetectionEnabled = false,
                    EdgeDetectionSettings = EdgeDetectionSettings.Default,

                    BloomEnabled = false,
                    BloomSettings = BloomSettings.Default,

                    ColorOverlapEnabled = false,
                    ColorOverlapSettings = ColorOverlapSettings.Default,
                };
            }
        }

        #endregion
    }
}
