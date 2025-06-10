using Domain;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Infrastructure
{
    public class BroadcastMessage : IBroadcastMessage
    {
        public void Broadcast(Message message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message.Json);

            Dictionary<WebSocket, bool> webSocketClients = new Dictionary<WebSocket, bool>();

            foreach (User u in message.Receiver.Participants)
            {
                foreach (var s in u.Sessions)
                {
                    webSocketClients.Add(s.Key, s.Value);
                }
            }

            lock (webSocketClients)
            {
                var clients = new List<WebSocket>(webSocketClients.Keys);
                foreach (var client in clients)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            client.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("\nError broadcasting to client: " + ex.Message);
                        }
                    }
                }
            }
        }
    }
}
