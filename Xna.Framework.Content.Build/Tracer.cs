#region Using

using System.Diagnostics;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class Tracer
    {
        static TraceSource traceSource = new TraceSource("Willcraftia.Xna.Framework.Content.Build");

        public static TraceSource TraceSource
        {
            get { return traceSource; }
        }
    }
}
