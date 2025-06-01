using Domain;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace Infrastructure
{
    public class BroadcastMessage : IBroadcastMessage
    {
        public void Broadcast(Dialogue receiver, string message, object lockObj)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            lock (lockObj)
            {
                foreach (var client in receiver.Participants)
                {
                    foreach (var session in client.Sessions)
                    {
                        try
                        {
                            if (session.Connected)
                            {
                                NetworkStream stream = session.GetStream();
                                stream.Write(data, 0, data.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("\nError broadcasting to client: " + ex.Message);
                        }
                    }
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
