using BtcWebSrvApp.API;
using BtcWebSrvApp.Business.Dapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BtcWebSrvApp.Business
{
    public class SendBtcQueue
    {
        static SendBtcQueue()
        {

        }

        static  ConcurrentQueue<Action> cq = new ConcurrentQueue<Action>();
        
        public static void EnqueueTask(Action t)
        {
            cq.Enqueue(t);
        }
        static object _locker = new object();
        public static void Start(int workers=1)
        {

            Action worker_action = () =>
            {
                Action taskAction;

                while (cq.TryDequeue(out taskAction))
                {
                    lock (_locker)
                    {
                        taskAction();
                    }
                }
            };

            var wrks = new Action[workers];

            for (var i = 0; i < workers; i++)
                wrks[i] = worker_action;

            

            while (true)
            {
                Parallel.Invoke(wrks);

                Thread.Sleep(1500);
            }

        }


    }
}
