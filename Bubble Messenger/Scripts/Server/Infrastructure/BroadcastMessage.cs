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

            Console.WriteLine(message.Id);

            List<WebSocket> webSocketClients = new List<WebSocket>();

            foreach (User u in message.Receiver.Participants)
            {
                foreach (var s in u.NewSessions)
                {
                    webSocketClients.Add(s);
                }
            }

            lock (webSocketClients)
            {
                foreach (var client in webSocketClients)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            Console.WriteLine("Message sent.");
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
