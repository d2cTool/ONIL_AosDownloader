using System;
using System.Threading;

namespace AosP2PClient.Common
{
    public static class ToolBox
    {
        public static void RaiseAsyncEvent<T>(EventHandler<T> e, object o, T args) where T : EventArgs
        {
            if (e == null)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(delegate { e(o, args); });
        }
    }
}
