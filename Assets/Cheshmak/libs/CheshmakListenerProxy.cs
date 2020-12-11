using UnityEngine;

namespace CheshmakMe
{
    public class CheshmakListenerProxy : AndroidJavaProxy
    {
        private ICheshmakListener listener;

        internal CheshmakListenerProxy(ICheshmakListener listener)
            : base("unity.cheshmak.wrapper.ICheshmakListener")
        {
            this.listener = listener;
        }

        void onCheshmakEvent(string type, string eventName, string paramString)
        {
           if (listener != null)
           {
                listener.onCheshmakEvent(type, eventName, paramString);
           }
        }

        public override string toString()
        {
            return "CheshmakListenerProxy";
        }
    }
}