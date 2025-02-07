// Made by MaxxTheLightning, 2025

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

class UnifiedServer
{
    private static readonly List<TcpClient> tcpClients = new List<TcpClient>();
    private static readonly Dictionary<WebSocket, bool> webSocketClients = new Dictionary<WebSocket, bool>();
    private static readonly object lockObj = new object();
    private static bool _isManaging = false;
    private static string ip_address = "";
    private static bool all_users_muted = false;
    private static List<string> banlist = new List<string>();
    private static List<string> banlist_reasons = new List<string>();
    private static bool _programIsStartable;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Made by MaxxTheLightning, 2025\n");
        Console.WriteLine("╔══╗─╔╗╔╗╔══╗─╔══╗─╔╗──╔═══╗───╔══╗╔══╗╔═══╗╔═══╗──\r\n║╔╗║─║║║║║╔╗║─║╔╗║─║║──║╔══╝───║╔═╝║╔╗║║╔═╗║║╔═╗║──\r\n║╚╝╚╗║║║║║╚╝╚╗║╚╝╚╗║║──║╚══╗───║║──║║║║║╚═╝║║╚═╝║──\r\n║╔═╗║║║║║║╔═╗║║╔═╗║║║──║╔══╝───║║──║║║║║╔╗╔╝║╔══╝──\r\n║╚═╝║║╚╝║║╚═╝║║╚═╝║║╚═╗║╚══╗───║╚═╗║╚╝║║║║║─║║───╔╗\r\n╚═══╝╚══╝╚═══╝╚═══╝╚══╝╚═══╝───╚══╝╚══╝╚╝╚╝─╚╝───╚╝\r\n");
        Console.WriteLine("Welcome to Bubble main server.\n");
        Console.WriteLine("WARNING: You must start the server with administrator rights!\n");
        Console.WriteLine("Commands:\n");
        Console.WriteLine("1. /say [ip-address] text --------------------- say anything in chat (ex.: say 12.34.56.78 Hello!)\n");
        Console.WriteLine("2. /spy --------------------------------------- Start managing messages\n");
        Console.WriteLine("3. /stop_spy ---------------------------------- Stop managing messages\n");
        Console.WriteLine("4. /mute [username] --------------------------- Mute user by nickname\n");
        Console.WriteLine("5. /muteall ----------------------------------- Mute everybody in current session\n");
        Console.WriteLine("6. /unmute [username] ------------------------- Unmute user by nickname\n");
        Console.WriteLine("7. /unmuteall --------------------------------- Unmute everybody in current session\n");
        Console.WriteLine("8. /mutelist ---------------------------------- Show the list of muted users\n");
        Console.WriteLine("Do you want to manage all messages? (y/n)\n");

        string choise = Console.ReadLine();         // Выбор контролирования сообщений в консоли сервера

        switch (choise)
        {
            case "y":       // yes
                Console.WriteLine("\nNow you're managing all messages.\n");
                _isManaging = true;
                break;
            case "n":       // no
                Console.WriteLine("\nNow you aren't managing all messages. If you want, you can use command 'spy'.\n");
                _isManaging = false;
                break;
            default:
                Console.WriteLine("\nError: Incorrect input. Defaulting to not managing messages.\n");
                _isManaging = false;
                break;
        }

        Console.WriteLine("Now you can start your server. Write IP-address:\n");
        ip_address = Console.ReadLine();        // IP-адрес, на котором будет запущен сервер

        Console.WriteLine("\nStarting server...\n");

        Task.Run(() => StartTcpChatServer());
        Task.Run(() => HandleConsoleInput());
        try
        {
            await StartWebSocketServer();       //  ERROR
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
                if (_isManaging)
                {
                    Console.WriteLine("\nMessage received: " + message);
                }

                // Broadcast to both TCP and WebSocket clients
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
        httpListener.Prefixes.Add($"http://{ip_address}:8080/");
        try
        {
            httpListener.Start();       //  ERROR
            Console.WriteLine($"WebSocket Server started on {ip_address}:8080...\n");
        }
        catch
        {
            Console.WriteLine("Failed to start the server. Check your IP-address and restart the server with administrator rights.");
        }

        while (true)
        {
            if(_programIsStartable)
            {
                var httpContext = await httpListener.GetContextAsync();
                if (httpContext.Request.IsWebSocketRequest)
                {
                    var wsContext = await httpContext.AcceptWebSocketAsync(null);
                    Console.WriteLine("\nWebSocket connection established.\n");
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
                Console.WriteLine("\nWebSocket connection closed.");
                lock (webSocketClients)
                {
                    webSocketClients.Remove(webSocket);
                }
            }
            else
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // Broadcast to both TCP and WebSocket clients
                bool isMuted = false;
                foreach (string i in banlist)
                {
                    string formatted_name = "{" + $"\"name\":\"{i}\"";
                    if (message.StartsWith(formatted_name))
                    {
                        isMuted = true;
                        break;
                    }
                }
                if (!isMuted && !all_users_muted)
                {
                    if (_isManaging)
                    {
                        Console.WriteLine("\nMessage received from client: " + message);
                    }
                    BroadcastMessage(message);
                }
            }
        }
    }

    private static void BroadcastMessage(string message)
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
                    if (_isManaging)
                    {
                        Console.WriteLine("\nError broadcasting to client: " + ex.Message);
                    }
                }
            }
        }

        // Broadcast to WebSocket clients
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
                        if (_isManaging)
                        {
                            Console.WriteLine("\nError broadcasting to client: " + ex.Message);
                        }
                    }
                }
            }
        }
    }

    static void HandleConsoleInput()
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (input.StartsWith("/say "))
            {
                string[] parts = input.Split(' ', 3);
                if (parts.Length == 3)
                {
                    string ip = parts[1];
                    string message = parts[2];
                    string timestamp = DateTime.Now.ToString("HH:mm:ss");
                    string formattedMessage = $"Server ({timestamp}): {message}";

                    string real_message = $"{message}";

                    Console.WriteLine("\nSending: " + formattedMessage);
                    BroadcastMessage(real_message);
                }
            }
            if (input.StartsWith("/mute "))
            {
                string[] parts = input.Split(' ', 3);
                if (parts.Length == 3)
                {
                    string user = parts[1];
                    string reason = parts[2];
                    bool isAlreadyBanned = false;
                    foreach (string i in banlist)
                    {
                        if (i == user)
                        {
                            isAlreadyBanned = true;
                            break;
                        }
                    }
                    if (!isAlreadyBanned)
                    {
                        banlist.Add(user);
                        banlist_reasons.Add(reason);
                        Console.WriteLine($"\n{user} muted successfully. Reason: {reason}\n");
                        BroadcastMessage($"{user} was muted by administrator. Reason: {reason}");
                    }
                    else
                    {
                        Console.WriteLine($"\n{user} is already muted.\n");
                    }
                }
            }
            if (input.StartsWith("/unmute "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string user = parts[1];
                    bool nowIsBanned = false;
                    foreach (string i in banlist)
                    {
                        if (i == user)
                        {
                            nowIsBanned = true;
                            break;
                        }
                    }
                    if (nowIsBanned)
                    {
                        banlist.Remove(user);
                        int index = banlist.IndexOf(user);
                        banlist_reasons.Remove(banlist_reasons[index]);
                        Console.WriteLine($"\n{user} unmuted successfully.\n");
                        BroadcastMessage($"{user} was unmuted by administrator.");
                    }
                    else
                    {
                        Console.WriteLine($"\n{user} is not muted.\n");
                    }
                }
            }
            if (input == "/mutelist")
            {
                if (banlist.Count > 0)
                {
                    Console.WriteLine("\n===============================");
                    Console.WriteLine($"Number of muted users: {banlist.Count}");
                    int user_number = 1;
                    foreach (string i in banlist)
                    {
                        Console.WriteLine($"{user_number}: {i}. Reason: {banlist_reasons[user_number - 1]}");
                        user_number++;
                    }
                    Console.WriteLine("===============================\n");
                }
                else
                {
                    Console.WriteLine("\nThere's no muted users yet.\n");
                }
            }
            if (input == "/muteall")
            {
                if (!all_users_muted)
                {
                    all_users_muted = true;
                    Console.WriteLine("\nNow nobody cannot send messages.\n");
                    BroadcastMessage("Everybody in this conversation was muted by administrator.");
                }
                else
                {
                    Console.WriteLine("\nAll users are already muted!\n");
                }
            }
            if (input == "/unmuteall")
            {
                if (banlist.Count > 0)
                {
                    int old_num_of_users = banlist.Count;
                    banlist.Clear();
                    banlist_reasons.Clear();
                    all_users_muted = false;
                    Console.WriteLine($"\nAll users from mutelist ({old_num_of_users}) was unmuted.\n");
                    BroadcastMessage($"All users from mutelist ({old_num_of_users}) was unmuted.");
                }
                else if (all_users_muted)
                {
                    all_users_muted = false;
                    Console.WriteLine("\nAll users was unmuted.\n");
                    BroadcastMessage("Everybody in this conversation was unmuted by administrator.");
                }
                else
                {
                    Console.WriteLine("\nThere's no muted users yet.\n");
                }
            }
            if (input == "/spy")
            {
                if (_isManaging)
                {
                    Console.WriteLine("\nError: You're already managing messages!");
                }
                else
                {
                    Console.WriteLine("\nNow you're managing all messages.");
                    _isManaging = true;
                }
            }
            if (input == "/stop_spy")
            {
                if (!_isManaging)
                {
                    Console.WriteLine("\nError: You're already not managing messages!");
                }
                else
                {
                    Console.WriteLine("\nNow you aren't managing all messages. If you want, you can use command 'spy'.");
                    _isManaging = false;
                }
            }
        }
    }
}