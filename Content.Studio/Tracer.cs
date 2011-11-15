#region Using

using System;
using System.Diagnostics;

#endregion

namespace Willcraftia.Content.Studio
{
    public sealed class Tracer
    {
        static TraceSource traceSource = new TraceSource("Willcraftia.Content.Studio");

        public static TraceSource TraceSource
        {
            get { return traceSource; }
        }
    }
}
