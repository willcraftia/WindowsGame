#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Willcraftia.Win.Framework.Diagnostics;

#endregion

namespace Willcraftia.Content.Studio
{
    public sealed class AppTraceListener : DelegateTraceListener
    {
        #region Fields and Properties

        static AppTraceListener current;
        public static AppTraceListener Current
        {
            get { return current; }
        }

        StringBuilder traceCache = new StringBuilder();
        public string TraceCache
        {
            get { return traceCache.ToString(); }
        }

        bool traceCacheEnabled = true;
        public bool TraceCacheEnabled
        {
            get { return traceCacheEnabled; }
            set { traceCacheEnabled = value; }
        }

        #endregion

        #region Constructors

        public AppTraceListener()
        {
            current = this;
        }

        #endregion

        #region Overrides

        public override void Write(string message)
        {
            if (traceCacheEnabled)
            {
                traceCache.Append(message);
            }

            base.Write(message);
        }

        public override void WriteLine(string message)
        {
            if (traceCacheEnabled)
            {
                traceCache.Append(message + Environment.NewLine);
            }

            base.WriteLine(message);
        }

        #endregion

        public string ConsumeTraceCache()
        {
            var result = traceCache.ToString();
            traceCache.Clear();
            return result;
        }
    }
}
