using System.Net.WebSockets;

namespace Domain
{
    public interface IBroadcastMessage
    {
        public void Broadcast(Dialogue receiver, string message, object lockObj, Dictionary<WebSocket, bool> webSocketClients);
    }
}
