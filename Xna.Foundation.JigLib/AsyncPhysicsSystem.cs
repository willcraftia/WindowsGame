#define USE_VOLATILE

#region Using

using System;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Xna.Framework;
using JigLibX.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// JigLibX の PhysicsSystem のインテグレーションを非同期実行するクラスです。
    /// </summary>
    public sealed class AsyncPhysicsSystem
    {
        PhysicsSystem physicsSystem;

        bool enabled;

        /// <summary>
        /// 非同期処理が有効かどうかを示す値。
        /// </summary>
        /// <value>
        /// trye (非同期処理が有効な場合)、false (それ以外の場合)。
        /// </value>
        /// <remarks>
        /// 非同期処理を有効にすると、非同期処理のためのスレッドを開始します。
        /// 一方、無効にすると、そのスレッドを停止します。
        /// </remarks>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;

                    if (enabled)
                    {
                        StartThread();
                    }
                    else
                    {
                        StopThread();
                    }
                }
            }
        }

#if XBOX
		int processorAffinity = 4;
		volatile bool exit;
#elif USE_VOLATILE
        volatile bool exit;
#else
        ManualResetEvent exitEvent = new ManualResetEvent(false);
        WaitHandle[] beginUpdateEvents;
        WaitHandle[] threadEvents;
#endif

        AutoResetEvent beginEvent = new AutoResetEvent(false);
        ManualResetEvent completedEvent = new ManualResetEvent(true);

        Thread thread;
        AsyncGameTime asyncGameTime = new AsyncGameTime();

        bool updated;

        float targetStepTime;
        public TimeSpan TargetStepTime
        {
            get { return TimeSpan.FromSeconds(targetStepTime); }
            set { targetStepTime = (float) value.TotalSeconds; }
        }

        float maximumStepTime = float.MaxValue;
        public TimeSpan MaximumStepTime
        {
            get { return TimeSpan.FromSeconds(maximumStepTime); }
            set { maximumStepTime = (float) value.TotalSeconds; }
        }

        float speed = 1.0f;

        /// <summary>
        /// Gets or sets the simulation speed scale of the physics processing.
        /// </summary>
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        Collection<IAsyncIntegrationListener> asyncIntegrationListeners = new Collection<IAsyncIntegrationListener>();
        public Collection<IAsyncIntegrationListener> AsyncIntegrationListeners
        {
            get { return asyncIntegrationListeners; }
        }

        public AsyncPhysicsSystem(Game game, PhysicsSystem physicsSystem)
        {
            this.physicsSystem = physicsSystem;

#if !XBOX && !USE_VOLATILE
			beginUpdateEvents = new WaitHandle[] { exitEvent, completedEvent };
			threadEvents = new WaitHandle[] { beginEvent, exitEvent };
#endif
            
            targetStepTime = game.IsFixedTimeStep ? (float) game.TargetElapsedTime.TotalSeconds : 1.0f / 60.0f;
        }

        public void BeginUpdate(GameTime gameTime)
        {
            if (enabled)
            {
                // Wait until signaled that a previous update has completed or if the thread has/will exit.
#if XBOX || USE_VOLATILE
                if (completedEvent.WaitOne() && !exit)
#else
				if (WaitHandle.WaitAny(beginUpdateEvents) != 0)
#endif
                {
                    // Not completed anymore.
                    completedEvent.Reset();

                    // Hold onto the game time for other thread.
                    asyncGameTime.totalGameTime = gameTime.TotalGameTime;
                    asyncGameTime.elapsedGameTime = gameTime.ElapsedGameTime;
                    asyncGameTime.isRunningSlowly = gameTime.IsRunningSlowly;

                    // Signal the other thread to process.
                    beginEvent.Set();
                }
            }
        }

        public void EndUpdate()
        {
            if (enabled)
            {
                // Wait until the update is complete.  This won't block if an update was never started.
                completedEvent.WaitOne();

                // Notify subscribers that the update is completed.
                if (updated)
                {
                    updated = false;
                }
            }
        }

        void StartThread()
        {
            // Reset the the exit event.
#if XBOX || USE_VOLATILE
			exit = false;
#else
            exitEvent.Reset();
#endif
            // Start the thread.
            thread = new Thread(ThreadRun);
            thread.Start();
        }

        void StopThread()
        {
            // Signal the exit event.
#if XBOX || USE_VOLATILE
			exit = true;
			beginEvent.Set();
#else
            exitEvent.Set();
#endif
            // Wait for the thread to abort.
            if (thread != null)
            {
                thread.Join();
            }
        }

        void ThreadRun()
        {
#if XBOX
			// Set the processor affinity.
			Thread.CurrentThread.SetProcessorAffinity(this.processorAffinity);

			// Wait until signaled to start updating or to exit.  An exit event will terminate the thread.
			while (beginEvent.WaitOne() && !exit)
#elif USE_VOLATILE
            // Wait until signaled to start updating or to exit.  An exit event will terminate the thread.
            while (beginEvent.WaitOne() && !exit)
#else
			// Wait until signaled to start updating or to exit.  An exit event will terminate the thread.
			// TODO : GC!
			while (WaitHandle.WaitAny(threadEvents) != 1)
#endif
            {
                // Update this component.
                Update(asyncGameTime);
                updated = true;

                // Signal completion.
                completedEvent.Set();
            }
        }

        void Update(AsyncGameTime gameTime)
        {
            // Get the elapsed time in seconds including fractions of a second.  Take into account
            // the simulation speed.
            var elapsed = (float) (gameTime.ElapsedGameTime.TotalSeconds * speed);

            // Accumulate the step time and account for the maxmimum.
            var stepTime = elapsed;
            if (stepTime > maximumStepTime)
            {
                stepTime = maximumStepTime;
            }

            // Get the speed adjusted destination step time.
            var adjustedTargetStepTime = targetStepTime * speed;

            // Make sure simulation keeps up with the frame rate.
            while (stepTime > 0.0f)
            {
                // Get the time step for the current iteration.
                var currentStepTime = Math.Min(stepTime, adjustedTargetStepTime);

                // Notify all listeners of the simulation step being started.
                if (asyncIntegrationListeners.Count != 0)
                {
                    foreach (var listener in asyncIntegrationListeners)
                    {
                        listener.PreIntegration(gameTime);
                    }
                }

                //var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
                //physicsSystem.Integrate(dt);
                physicsSystem.Integrate(currentStepTime);

                // Notify all listeners of the simulation step being finished.
                if (asyncIntegrationListeners.Count != 0)
                {
                    foreach (var listener in asyncIntegrationListeners)
                    {
                        listener.PostIntegration(gameTime);
                    }
                }

                // Update the time step to see if another step is necessary for the update.
                stepTime -= adjustedTargetStepTime;
            }
        }
    }
}
