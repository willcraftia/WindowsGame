using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Willcraftia.Xna.Foundation.Content.Pipeline.TerrainMap
{
    public sealed class TerrainMapAltitudeColorDescription
    {
        public int MinAltitude;
        public int MaxAltitude;
        public Color MinColor;
        public Color MaxColor;
        public Color? MinBorderColor;
        public Color? MaxBorderColor;

        public void Validate()
        {
            if (MinAltitude < 0)
            {
                throw new InvalidContentException("Min altitude must be a nonnegative value.");
            }
            if (MaxAltitude < 0 || MaxAltitude < MinAltitude)
            {
                throw new InvalidContentException("Min altitude must be a nonnegative value and greater than min altitude.");
            }
        }

        public bool Contains(int altitude)
        {
            return MinAltitude <= altitude && altitude <= MaxAltitude;
        }

        public void ResolveColor(int altitude, out Color result)
        {
            if (MinAltitude == altitude)
            {
                result = MinBorderColor ?? MinColor;
                return;
            }
            if (MaxAltitude == altitude)
            {
                result = MaxBorderColor ?? MaxColor;
                return;
            }

            var minVector = MinColor.ToVector3();
            var maxVector = MaxColor.ToVector3();

            float s = ((float) altitude - (float) MinAltitude) / ((float) MaxAltitude - (float) MinAltitude);
            var vectorColor = minVector + s * (maxVector - minVector);
            result = new Color(vectorColor);
        }
    }
}
