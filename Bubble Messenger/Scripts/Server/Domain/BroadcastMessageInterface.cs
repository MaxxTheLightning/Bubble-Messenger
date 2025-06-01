namespace Domain
{
    public interface IBroadcastMessage
    {
        public void Broadcast(Dialogue receiver, string message, object lockObj);
    }
}
