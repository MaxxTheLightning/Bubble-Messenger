using System.Net.WebSockets;

namespace Domain
{
    public interface IBroadcastMessage
    {
        public void Broadcast(Message message);
    }
}
