// Uncomment this if you want to measure the time actions take to execute and report slow ones
//#define WRAP_AND_MEASURE_ACT_WITH_STOPWATCH

using System;
using System.Collections.Concurrent;

namespace ACE.Server.Entity.Actions
{
    public class ActionQueue : IActor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected ConcurrentQueue<IAction> Queue { get; } = new ConcurrentQueue<IAction>();

        #if WRAP_AND_MEASURE_ACT_WITH_STOPWATCH
        private readonly System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        #endif

        public void RunActions()
        {
            if (Queue.IsEmpty)
                return;

            var count = Queue.Count;

            for (int i = 0; i < count; i++)
            {
                if (Queue.TryDequeue(out var result))
                {
                    #if WRAP_AND_MEASURE_ACT_WITH_STOPWATCH
                    sw.Restart();
                    #endif

                    try
                    {
                        Tuple<IActor, IAction> enqueue = result.Act();

                        #if WRAP_AND_MEASURE_ACT_WITH_STOPWATCH
                        sw.Stop();

                        if (sw.Elapsed.TotalSeconds > 1)
                        {
                            if (result is ActionEventDelegate actionEventDelegate)
                            {
                                if (actionEventDelegate.Action.Target is WorldObjects.WorldObject worldObject)
                                    log.Warn($"ActionQueue Act() took {sw.Elapsed.TotalSeconds:N0}s. Method.Name: {actionEventDelegate.Action.Method.Name}, Target: {actionEventDelegate.Action.Target} 0x{worldObject.Guid}:{worldObject.Name}");
                                else
                                    log.Warn($"ActionQueue Act() took {sw.Elapsed.TotalSeconds:N0}s. Method.Name: {actionEventDelegate.Action.Method.Name}, Target: {actionEventDelegate.Action.Target}");
                            }
                            else
                                log.Warn($"ActionQueue Act() took {sw.Elapsed.TotalSeconds:N0}s.");
                        }
                        #endif

                        if (enqueue != null)
                            enqueue.Item1.EnqueueAction(enqueue.Item2);
                    }
                    catch (Exception ex)
                    {
                        // A single queued action must never take down the world thread (or a landblock's action queue).
                        // Log the offending action and continue processing the rest of the queue.
                        if (result is ActionEventDelegate aed)
                            log.Error($"[TICK_EXCEPTION] ActionQueue action aborted. Method.Name: {aed.Action?.Method?.Name}, Target: {aed.Action?.Target}", ex);
                        else
                            log.Error($"[TICK_EXCEPTION] ActionQueue action aborted.", ex);
                    }
                }
            }
        }

        public void EnqueueAction(IAction action)
        {
            Queue.Enqueue(action);
        }

        public void Clear()
        {
            Queue.Clear();
        }
    }
}
