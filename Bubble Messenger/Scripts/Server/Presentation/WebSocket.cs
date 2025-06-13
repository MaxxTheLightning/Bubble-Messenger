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
        IUserRepo UserRepo { get; }

        public WebSocketSpace(IUserRepo userRepo)
        {
            UserRepo = userRepo;
        }

        private static readonly List<WebSocket> webSocketClients = new List<WebSocket>();

        public void Start()
        {
            Task.Run(() => Main(UserRepo));
        }

        static async Task Main(IUserRepo userRepo)
        {
            try
            {
                await StartWebSocketServer(userRepo);
            }
            catch
            {
                return;
            }
        }

        private static async Task StartWebSocketServer(IUserRepo userRepo)
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

                    _ = HandleWebSocketClient(wsContext.WebSocket, userRepo);
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                    httpContext.Response.Close();
                }
            }
        }

        private static async Task HandleWebSocketClient(WebSocket webSocket, IUserRepo userRepo)
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

                    Console.WriteLine($"\nNew message received: {message}");

                    string _userId = ParseJson(message, "user_id");

                    string _action = ParseJson(message, "action");

                    User user = userRepo.GetUserById(_userId);

                    if (_action == "connected")
                    {
                        user.NewSessions.Add(webSocket);

                        Console.WriteLine($"\n{user.Name} opened a new session.");
                    }
                    else if (_action == "disconnected")
                    {
                        user.NewSessions.Remove(webSocket);

                        Console.WriteLine($"\n{user.Name} closed a session.");
                    }
                }
            }
        }

        private static string ParseJson(string json, string value_name)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            string result = doc.RootElement.GetProperty(value_name).GetString();
            return result;
        }
    }
}
