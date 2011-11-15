#region Using

using System;
using System.Diagnostics;
using Microsoft.Build.Framework;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class BuildLogger : ILogger
    {
        public string Parameters { get; set; }

        public LoggerVerbosity Verbosity { get; set; }

        public TraceSource TraceSource { get; private set; }

        public BuildLogger(TraceSource traceSource)
        {
            TraceSource = traceSource;
            Verbosity = LoggerVerbosity.Normal;
        }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.BuildStarted += new BuildStartedEventHandler(OnBuildStarted);
            eventSource.BuildFinished += new BuildFinishedEventHandler(OnBuildFinished);
            eventSource.ErrorRaised += new BuildErrorEventHandler(OnErrorRaised);
            eventSource.WarningRaised += new BuildWarningEventHandler(OnWarningRaised);
            //eventSource.MessageRaised += new BuildMessageEventHandler(OnMessageRaised);
            eventSource.ProjectStarted += new ProjectStartedEventHandler(OnProjectStarted);
            eventSource.ProjectFinished += new ProjectFinishedEventHandler(OnProjectFinished);
            //eventSource.TargetStarted += new TargetStartedEventHandler(OnTargetStarted);
            //eventSource.TargetFinished += new TargetFinishedEventHandler(OnTargetFinished);
            //eventSource.TaskStarted += new TaskStartedEventHandler(OnTaskStarted);
            //eventSource.TaskFinished += new TaskFinishedEventHandler(OnTaskFinished);
            //eventSource.StatusEventRaised += new BuildStatusEventHandler(OnStatusEventRaised);
        }

        void OnBuildStarted(object sender, BuildStartedEventArgs e)
        {
            TraceSource.TraceInformation(e.Message);
        }

        void OnBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            TraceSource.TraceInformation(e.Message);
        }

        void OnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            TraceSource.TraceEvent(TraceEventType.Error, 0, e.Message);
        }

        void OnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            TraceSource.TraceEvent(TraceEventType.Warning, 0, e.Message);
        }

        //void OnMessageRaised(object sender, BuildMessageEventArgs e)
        //{
        //    traceSource.TraceInformation(e.Message);
        //}

        void OnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            TraceSource.TraceInformation(e.Message);
        }

        void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            TraceSource.TraceInformation(e.Message);
        }

        //void OnTargetStarted(object sender, TargetStartedEventArgs e)
        //{
        //    traceSource.TraceInformation(e.Message);
        //}

        //void OnTargetFinished(object sender, TargetFinishedEventArgs e)
        //{
        //    traceSource.TraceInformation(e.Message);
        //}

        //void OnTaskStarted(object sender, TaskStartedEventArgs e)
        //{
        //    traceSource.TraceInformation(e.Message);
        //}

        //void OnTaskFinished(object sender, TaskFinishedEventArgs e)
        //{
        //    traceSource.TraceInformation(e.Message);
        //}

        //void OnStatusEventRaised(object sender, BuildStatusEventArgs e)
        //{
        //    traceSource.TraceInformation(e.Message);
        //}

        public void Shutdown()
        {
        }
    }
}
