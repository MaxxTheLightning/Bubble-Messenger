// Made by MaxxTheLightning, 2025

using System.Text.Json;
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
    class User
    {
        public string Name { get; set; }
        public bool isMuted { get; set; }
        public bool isOnline { get; set; }

        public User(string name)
        {
            Name = name;
            isOnline = true;
            isMuted = false;
        }
    }

    private static readonly List<TcpClient> tcpClients = new List<TcpClient>();
    private static readonly Dictionary<WebSocket, bool> webSocketClients = new Dictionary<WebSocket, bool>();
    private static readonly object lockObj = new object();
    private static bool _isManaging = false;
    private static string ip_address = "";
    private static bool all_users_muted = false;

    private static List<User> users = new List<User>();

    private static List<string> banlist = new List<string>();
    private static List<string> banlist_reasons = new List<string>();
    private static List<string> administrators = new List<string>();
    private static bool _programIsStartable = true;
    private static int _maxNumberOfUsers = 0;
    private static bool _unlimitedUsers = false;
    private static int _usersOnline = 0;

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
        Console.WriteLine("9. /max_users [number] ------------------------ Change the max number of users on server. (0 for unlimited)\n");
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

        Console.WriteLine("Enter max number of users on server (0 for unlimited):\n");

        int response = Convert.ToInt32(Console.ReadLine());
        if(response > 0)
        {
            _maxNumberOfUsers = response;
            Console.WriteLine($"\nMax number of users: {_maxNumberOfUsers}");
        }
        else if(response == 0)
        {
            _unlimitedUsers = true;
            Console.WriteLine("\nMax number of users: unlimited");
        }
        else
        {
            _unlimitedUsers = true;
            Console.WriteLine("\nError! Defaulting to unlimited max number of users.");
        }

        Console.WriteLine("\nNow you can start your server. Write IP-address:\n");
        ip_address = Console.ReadLine();        // IP-адрес, на котором будет запущен сервер

        Console.WriteLine("\nStarting server...\n");

        Task.Run(() => StartTcpChatServer());
        Task.Run(() => HandleConsoleInput());
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
            if(_programIsStartable && ((_usersOnline < _maxNumberOfUsers) || _unlimitedUsers))
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

                string type = ParseJson(message, "type");

                if (type == "info")
                {
                    string text = ParseJson(message, "text");
                    string name = ParseJson(message, "name");
                    string time = ParseJson(message, "time");

                    if (text == "connected to server")
                    {
                        bool isAlreadyUser = false;
                        foreach (User current in users)
                        {
                            if (current.Name == name)
                            {
                                current.isOnline = true;
                                isAlreadyUser = true;
                                Console.WriteLine($"{current.Name} now is online.");
                                break;
                            }
                        }
                        if (!isAlreadyUser)
                        {
                            users.Add(new User(name));
                            Console.WriteLine($"New user ({name}) added.");
                        }
                    }

                    else if (text == "disconnected from server")
                    {
                        foreach (User current in users)
                        {
                            if (current.Name == name)
                            {
                                current.isOnline = false;
                                Console.WriteLine($"{current.Name} left the conversation");
                                break;
                            }
                        }
                    }
                }
                else if (type == "chat")
                {
                    // Broadcast to both TCP and WebSocket clients

                    bool isMuted = false;

                    string name = ParseJson(message, "name");

                    foreach (User current in users)
                    {
                        if (current.Name == name)
                        {
                            if (current.isMuted)
                            {
                                isMuted = true;
                            }
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
    }

    private static string ParseJson(string json, string value_name)
    {
        using JsonDocument doc = JsonDocument.Parse(json);
        string result = doc.RootElement.GetProperty(value_name).GetString();
        return result;
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
            if(input.StartsWith("/max_users "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    int new_max_users = Convert.ToInt32(parts[1]);
                    if(new_max_users > 0)
                    {
                        _maxNumberOfUsers = new_max_users;
                        _unlimitedUsers = false;
                        Console.WriteLine($"\nNew max number of users: {_maxNumberOfUsers}\n");
                    }
                    else if(new_max_users == 0)
                    {
                        _unlimitedUsers = true;
                        Console.WriteLine("\nNew max number of users: unlimited\n");
                    }
                    else
                    {
                        Console.WriteLine($"\nInput error!\n");
                    }
                }
            }
            if (input.StartsWith("/say "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string message = parts[1];
                    BroadcastMessage(message);
                }
            }
            if (input.StartsWith("/mute "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string user = parts[1];
                    foreach (User current in users)
                    {
                        if (current.Name == user)
                        {
                            if (!current.isMuted)
                            {
                                current.isMuted = true;
                                Console.WriteLine($"\n{user} muted successfully.\n");
                                BroadcastMessage($"{user} was muted by administrator.");
                            }
                            else
                            {
                                Console.WriteLine($"\n{user} is already muted.\n");
                            }
                            break;
                        }
                    }
                }
            }
            if (input.StartsWith("/unmute "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string user = parts[1];
                    foreach (User current in users)
                    {
                        if (current.Name == user)
                        {
                            if (current.isMuted)
                            {
                                current.isMuted = false;
                                Console.WriteLine($"\n{user} unmuted successfully.\n");
                                BroadcastMessage($"{user} was unmuted by administrator.");
                            }
                            else
                            {
                                Console.WriteLine($"\n{user} is not muted.\n");
                            }
                            break;
                        }
                    }
                }
            }
            if (input == "/mutelist")
            {
                /*if (banlist.Count > 0)
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
                }*/
                int muted_users = 0;
                List<string> usernames = new List<string>();

                foreach (User current in users)
                {
                    if (current.isMuted)
                    {
                        muted_users++;
                        usernames.Add(current.Name);
                    }
                }

                if (muted_users > 0)
                {
                    Console.WriteLine("Muted users:");
                    for (int i = 1; i <= muted_users; i++)
                    {
                        Console.WriteLine($"{i}. {usernames[i - 1]}");
                    }
                }

                else
                {
                    Console.WriteLine("There's no muted users yet.");
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