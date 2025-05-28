using Domain;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Presentation
{
    public class WebSocketSpace
    {
        private static readonly List<TcpClient> tcpClients = new List<TcpClient>();
        private static readonly Dictionary<WebSocket, bool> webSocketClients = new Dictionary<WebSocket, bool>();
        private static readonly object lockObj = new object();

        public void Start()
        {
            Task.Run(() => Main());
        }

        static async Task Main()
        {
            Task.Run(() => StartTcpChatServer());

            try
            {
                await StartWebSocketServer();
            }
            catch
            {
                return;
            }
        }

        private static async Task StartTcpChatServer()
        {
            TcpListener tcpServer = new TcpListener(IPAddress.Any, 5002);
            tcpServer.Start();
            Console.WriteLine("Server started.\n");

            while (true)
            {
                TcpClient client = await tcpServer.AcceptTcpClientAsync();
                lock (lockObj)
                {
                    tcpClients.Add(client);
                }
                Console.WriteLine("\nClient connected.");
                _ = HandleTcpClient(client);
            }
        }

        private static async Task HandleTcpClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (client.Connected)
            {
                try
                {
                    int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (byteCount == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    
                    Console.WriteLine("\nMessage received: " + message);

                    BroadcastMessage(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nError handling client: " + ex.Message);
                    break;
                }
            }

            lock (lockObj)
            {
                tcpClients.Remove(client);
            }

            client.Close();
            Console.WriteLine("\nClient disconnected.");
        }

        private static async Task StartWebSocketServer()
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://127.0.0.1:8080/");
            try
            {
                httpListener.Start();
                Console.WriteLine($"WebSocket Server started on 127.0.0.1:8080...\n");
            }
            catch
            {
                Console.WriteLine("Failed to start the server. Check your IP-address and restart the server with administrator rights.");
            }

            while (true)
            {
                var httpContext = await httpListener.GetContextAsync();
                if (httpContext.Request.IsWebSocketRequest)
                {
                    var wsContext = await httpContext.AcceptWebSocketAsync(null);
                    lock (webSocketClients)
                    {
                        webSocketClients[wsContext.WebSocket] = true;
                    }
                    _ = HandleWebSocketClient(wsContext.WebSocket);
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                    httpContext.Response.Close();
                }
            }
        }

        private static async Task HandleWebSocketClient(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    lock (webSocketClients)
                    {
                        webSocketClients.Remove(webSocket);
                    }
                }
                else
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    Console.WriteLine($"New message: {message}");

                    BroadcastMessage(message);
                }
            }
        }

        private static string ParseJson(string json, string value_name)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            string result = doc.RootElement.GetProperty(value_name).GetString();
            return result;
        }

        private static void BroadcastMessage(string message, string specialization = "")
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            lock (lockObj)
            {
                foreach (var client in tcpClients)
                {
                    try
                    {
                        if (client.Connected)
                        {
                            NetworkStream stream = client.GetStream();
                            stream.Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\nError broadcasting to client: " + ex.Message);
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
