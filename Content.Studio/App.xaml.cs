#region Using

using System.Diagnostics;
using System.Windows.Threading;
using System.Windows;

#endregion

namespace Willcraftia.Content.Studio
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Tracer.TraceSource.TraceEvent(TraceEventType.Critical, 0, e.Exception.Message + "\n" + e.Exception.StackTrace);
        }
    }
}
