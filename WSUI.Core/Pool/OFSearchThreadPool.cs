using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Nest;
using OF.Core.Extensions;
using OF.Core.Interfaces;

namespace OF.Core.Pool
{
    public class OFSearchThreadPool : IOFSearchThreadPool
    {
        
        #region [needs]
        
        private Thread internalThread = null;
        
        
        private readonly Queue<OFWorkThread> workingThreads = new Queue<OFWorkThread>();
        private readonly ConcurrentQueue<Action> workingActions = new ConcurrentQueue<Action>();
        private readonly AutoResetEvent waitActions = new AutoResetEvent(false);
        private readonly object lookObject = new object();


        #endregion
        

        public OFSearchThreadPool()
        {
            for (int i = 0; i <= 10; i++)
            {
                workingThreads.Enqueue(new OFWorkThread(this));
            }
            workingThreads.ForEach(t => t.Start());
            internalThread = new Thread(DoWork) {IsBackground =  true, Priority = ThreadPriority.Highest};
            internalThread.Start();
        }

        #region [public]

        void IOFSearchThreadPool.AddAction(Action action)
        {
            workingActions.Enqueue(action);
            waitActions.Set();
        }

        internal void PutThreadBack(OFWorkThread thread)
        {
            lock (this.lookObject)
            {
                workingThreads.Enqueue(thread);
            }
        }

        #endregion

        #region [private]

        private void DoWork()
        {
            do
            {
                waitActions.WaitOne();
                Action action = null;

                do
                {
                    if (workingActions.TryDequeue(out action))
                    {
                        OFWorkThread workThread = null;
                        lock (this.lookObject)
                            workThread = workingThreads.Dequeue();
                        workThread?.SetAction(action);
                    }

                } while (action != null);


            } while (true);
        }


        #endregion


    }
}