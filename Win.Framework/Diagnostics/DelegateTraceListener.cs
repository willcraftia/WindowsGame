#region Using

using System;
using System.Diagnostics;

#endregion

namespace Willcraftia.Win.Framework.Diagnostics
{
    public delegate void DelegateDraw(string message);

    public class DelegateTraceListener : TraceListener
    {
        public DelegateDraw DelegateDraw;

        public override void Write(string message)
        {
            if (DelegateDraw != null)
            {
                DelegateDraw(message);
            }
        }

        public override void WriteLine(string message)
        {
            if (DelegateDraw != null)
            {
                DelegateDraw(message + Environment.NewLine);
            }
        }
    }
}
