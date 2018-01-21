using System;
using System.Threading;
using OF.Core.Enums;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Core.Pool
{
    internal class OFWorkThread : IOFWorkThread
    {

        #region [needs]

        private Thread internalThreadl;

        private Action action;
        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);
        private volatile bool stop = false;
        private OFSearchThreadPool parentPool;

        #endregion

        internal OFWorkThread(OFSearchThreadPool pool)
            :this(ThreadPriority.Highest,pool)
        {
        }

        internal OFWorkThread(ThreadPriority priority,OFSearchThreadPool pool)
        {
            this.parentPool = pool;
            this.State = OFWorkThreadState.None;
            this.internalThreadl = new Thread(DoWork) {IsBackground = true,Priority = priority};
            
        }


        #region [internal]

        internal void Start()
        {
            internalThreadl.Start();
        }


        internal void Stop()
        {
            this.stop = true;
        }


        internal OFWorkThreadState State
        {
            get;
            private set;
        }


        internal void SetAction(Action action)
        {
            this.action = action;
            waitEvent.Set();
        }

        #endregion

        #region [private]
        private void DoWork()
        {
            while (!stop)
            {
                State = OFWorkThreadState.Waiting;
                waitEvent.WaitOne();
                State = OFWorkThreadState.Working;
                try
                {
                    if (this.action != null)
                    {
                        this.action();
                        this.action = null;
                    }
                    parentPool.PutThreadBack(this);
                }
                catch (Exception e)
                {
                    OFLogger.Instance.LogError(e.ToString());
                }
            }
            State = OFWorkThreadState.Stoped;
        }

        #endregion








    }
}