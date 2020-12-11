namespace CheshmakMe
{
    public interface ICheshmakListener
    {
        void onCheshmakEvent(string type,string eventName,string paramString);
    }
}