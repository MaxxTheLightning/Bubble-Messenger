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
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;
using System.Timers;
using System.Security.Cryptography.X509Certificates;

class UnifiedServer
{
    class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Bio { get; set; }
        public int SentMessages { get; set; }
        public bool isMuted { get; set; }
        public bool isBanned { get; set; }
        public bool isOnline { get; set; }
        public bool isAdmin { get; set; }
        public User(string name, string password, string bio, int sent_msgs, bool is_muted, bool is_banned, bool is_admin)
        {
            Name = name;
            Password = password;
            Bio = bio;
            SentMessages = sent_msgs;
            isOnline = false;
            isMuted = is_muted;
            isBanned = is_banned;
            isAdmin = is_admin;
            SetJson();
        }

        public string Json { get; set; }

        public void SetJson()
        {
            string json = "{" +
                $"\"name\": \"{Name}\"," +
                $"\"password\": \"{Password}\"," +
                $"\"bio\": \"{Bio}\"," +
                $"\"sent_msgs\": \"{SentMessages}\"," +
                $"\"isOnline\": \"False\"," +
                $"\"isMuted\": \"{isMuted}\"," +
                $"\"isBanned\": \"{isBanned}\"," +
                $"\"isAdmin\": \"{isAdmin}\"" +
                "}";
            Json = Regex.Replace(json, "\"", "\\\"");
        }
    }

    class Message
    {
        public string Sender { get; set; }
        public string Text { get; set; }

        public string To { get; set; }
        public string TimeStamp { get; set; }
        public string Json {  get; set; }
        public bool IsDeleted { get; set; }

        public Message(string sender, string to, string text, string time, string json)
        {
            Sender = sender;
            To = to;
            Text = text;
            TimeStamp = time;
            Json = json;
        }
    }

    private static readonly List<TcpClient> tcpClients = new List<TcpClient>();
    private static readonly Dictionary<WebSocket, bool> webSocketClients = new Dictionary<WebSocket, bool>();
    private static readonly object lockObj = new object();
    private static bool _isManaging = false;
    private static string ip_address = "";

    private static List<User> users = new List<User>();
    private static List<Message> messages = new List<Message>();

    private static bool _programIsStartable = true;
    private static int _maxNumberOfUsers = 0;
    private static bool _unlimitedUsers = false;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Made by MaxxTheLightning, 2025\n");
        Console.WriteLine("╔══╗─╔╗╔╗╔══╗─╔══╗─╔╗──╔═══╗───╔══╗╔══╗╔═══╗╔═══╗──\r\n║╔╗║─║║║║║╔╗║─║╔╗║─║║──║╔══╝───║╔═╝║╔╗║║╔═╗║║╔═╗║──\r\n║╚╝╚╗║║║║║╚╝╚╗║╚╝╚╗║║──║╚══╗───║║──║║║║║╚═╝║║╚═╝║──\r\n║╔═╗║║║║║║╔═╗║║╔═╗║║║──║╔══╝───║║──║║║║║╔╗╔╝║╔══╝──\r\n║╚═╝║║╚╝║║╚═╝║║╚═╝║║╚═╗║╚══╗───║╚═╗║╚╝║║║║║─║║───╔╗\r\n╚═══╝╚══╝╚═══╝╚═══╝╚══╝╚═══╝───╚══╝╚══╝╚╝╚╝─╚╝───╚╝\r\n");
        Console.WriteLine("Welcome to Bubble main server.\n");
        Console.WriteLine("WARNING: You must start the server with administrator rights!\n");
        Console.WriteLine("Commands:\n");
        Console.WriteLine("1. /say [text] -------------------------------- say anything in chat (ex.: /say Hello!)\n");
        Console.WriteLine("2. /spy --------------------------------------- Start managing messages\n");
        Console.WriteLine("3. /stop_spy ---------------------------------- Stop managing messages\n");
        Console.WriteLine("4. /mute [username] --------------------------- Mute user by nickname\n");
        Console.WriteLine("5. /muteall ----------------------------------- Mute everybody in current session\n");
        Console.WriteLine("6. /unmute [username] ------------------------- Unmute user by nickname\n");
        Console.WriteLine("7. /unmuteall --------------------------------- Unmute everybody in current session\n");
        Console.WriteLine("8. /ban [username] ---------------------------- Ban user by nickname\n");
        Console.WriteLine("9. /unban [username] -------------------------- Unban user by nickname\n");
        Console.WriteLine("10. /promote [username] ----------------------- Add administrator rights by nickname\n");
        Console.WriteLine("11. /dismiss [username] ----------------------- Remove administrator rights by nickname\n");
        Console.WriteLine("12. /mutelist --------------------------------- Show the list of muted users\n");
        Console.WriteLine("13. /max_users [number] ----------------------- Change the max number of users on server. (0 for unlimited)\n");
        Console.WriteLine("14. /users ------------------------------------ See all users on this server.\n");
        Console.WriteLine("15. /user [username] -------------------------- See all info about user.\n");
        Console.WriteLine("Do you want to manage all messages? (y/n)\n");

        string choise = Console.ReadLine();         // Выбор контролирования сообщений в консоли сервера

        switch (choise)
        {
            case "y":       // yes
                Console.WriteLine("\nNow you're managing all messages.\n");
                _isManaging = true;
                break;
            case "n":       // no
                Console.WriteLine("\nNow you aren't managing all messages. If you want, you can use command '/spy'.\n");
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
                BroadcastMessage("chat", message);  
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
        httpListener.Prefixes.Add($"http://{ip_address}:443/");
        try
        {
            httpListener.Start();       //  ERROR
            Console.WriteLine($"WebSocket Server started on {ip_address}:8080...\n");
            ReadHistory();
            ReadAccounts();
        }
        catch
        {
            Console.WriteLine("Failed to start the server. Check your IP-address and restart the server with administrator rights.");
        }

        while (true)
        {
            if(_programIsStartable && ((CalculateOnlineUsers() < _maxNumberOfUsers) || _unlimitedUsers))
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

                    if (text == "load history")
                    {
                        string opponent = ParseJson(message, "opponent");
                        foreach (Message current in messages)
                        {
                            if ((current.Sender == name && current.To == opponent) || (current.Sender == opponent && current.To == name) || (opponent == "main-chat" && current.To == "main-chat"))
                            {
                                BroadcastMessage("history", current.Json, name);
                            }
                        }
                    }

                    if (text == "connected to server")
                    {
                        bool exist = false;
                        foreach (User current in users)
                        {
                            if (current.Name == name)
                            {
                                exist = true;
                                current.isOnline = true;
                                Console.WriteLine($"{current.Name} now is online.");
                                BroadcastMessage("new connect", current.Name);
                                foreach (Message current_msg in messages)
                                {
                                    if (current_msg.To == "main-chat")
                                    {
                                        BroadcastMessage("history", current_msg.Json, current.Name);
                                    }
                                }
                                foreach (User this_user in users)
                                {
                                    BroadcastMessage("users-history", $"{this_user.Name} {this_user.isOnline}", current.Name);
                                }
                                break;
                            }
                        }

                        if (!exist)
                        {
                            BroadcastMessage("error", "Not registered");
                        }
                    }

                    else if (text == "disconnected from server")
                    {
                        foreach (User current in users)
                        {
                            if (current.Name == name)
                            {
                                current.isOnline = false;
                                BroadcastMessage("user_disconnected", name);
                                Console.WriteLine($"{current.Name} left the conversation");
                                break;
                            }
                        }
                    }

                    else if (text == "created account")
                    {
                        string password = ParseJson(message, "password");

                        users.Add(new User(name, password, "No bio...", 0, false, false, false));
                        Console.WriteLine($"New account ({name}) added.");
                    }

                    else if (text == "trying to connect")
                    {
                        string password = ParseJson(message, "password");
                        bool exist = false;
                        foreach (User current in users)
                        {
                            if (current.Name == name)
                            {
                                exist = true;
                                if (current.Password == password)
                                {
                                    Console.WriteLine($"User {name} loginned successfully.");
                                    BroadcastMessage("info", "Correct.");
                                }
                                else
                                {
                                    BroadcastMessage("info", "Invalid password. Please try again.");
                                }
                                break;
                            }
                        }

                        if (!exist)
                        {
                            Console.WriteLine($"Account \"{name}\" doesn't exist!");
                            BroadcastMessage("info", "Account doesn't exist!");
                        }
                    }
                }
                else if (type == "chat")
                {
                    // Broadcast to both TCP and WebSocket clients

                    bool isMuted = false;

                    string from = ParseJson(message, "from");
                    string to = ParseJson(message, "to");
                    string text = ParseJson(message, "text");
                    string time = ParseJson(message, "time");
                    string filetype = ParseJson(message, "filetype");

                    if (filetype == "text")
                    {
                        //  Проверяем, может ли пользователь отправлять сообщения

                        foreach (User current in users)
                        {
                            if (current.Name == from)
                            {
                                if (current.isMuted)
                                {
                                    isMuted = true;
                                }
                                else
                                {
                                    current.SentMessages++;
                                    string escapedMessage = Regex.Replace(message, "\"", "\\\"");
                                    Console.WriteLine(ParseJson(message, "to"));
                                    messages.Add(new Message(current.Name, ParseJson(message, "to"), text, time, escapedMessage));
                                    SaveHistory();
                                }
                                break;
                            }
                        }

                        if (!isMuted)
                        {
                            if (_isManaging)
                            {
                                Console.WriteLine("\nMessage received from client: " + message);
                            }
                            BroadcastMessage("chat", message);
                        }
                    }
                }
            }
        }
    }

    private static int CalculateOnlineUsers()
    {
        int result = 0;
        foreach (User current in users)
        {
            if (current.isOnline)
            {
                result++;
            }
        }
        return result;
    }

    private static string ParseJson(string json, string value_name)
    {
        using JsonDocument doc = JsonDocument.Parse(json);
        string result = doc.RootElement.GetProperty(value_name).GetString();
        return result;
    }

    private static void BroadcastMessage(string type, string message, string specialization = "")
    {
        byte[] data = null;

        if (type == "info" || type == "error" || type == "new connect" || type == "user_disconnected")
        {
            data = Encoding.UTF8.GetBytes("{" + $"\"type\" : \"{type}\", \"text\" : \"{message}\"" + "}");
        }
        else if (type == "history" || type == "users-history")
        {
            data = Encoding.UTF8.GetBytes("{" + $"\"type\" : \"{type}\", \"text\" : \"{message}\", \"specialization\" : \"{specialization}\"" + "}");
        }
        else if (type == "chat")
        {
            data = Encoding.UTF8.GetBytes(message);
        }
        

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

            if (input.StartsWith("/max_users "))
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
                    BroadcastMessage("chat", message);
                }
            }
            if (input.StartsWith("/mute "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string user = parts[1];
                    bool exist = false;
                    foreach (User current in users)
                    {
                        if (current.Name == user)
                        {
                            exist = true;
                            if (!current.isMuted)
                            {
                                current.isMuted = true;
                                Console.WriteLine($"\n{user} muted successfully.\n");
                                BroadcastMessage("chat", $"{user} was muted by administrator.");
                            }
                            else
                            {
                                Console.WriteLine($"\n{user} is already muted.\n");
                            }
                            break;
                        }
                    }
                    if (!exist)
                    {
                        Console.WriteLine($"There's no user named {user}");
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
                                BroadcastMessage("chat", $"{user} was unmuted by administrator.");
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
            if (input.StartsWith("/user "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string username = parts[1];
                    bool exist = false;
                    foreach (User current in users)
                    {
                        if (current.Name == username)
                        {
                            exist = true;
                            Console.WriteLine("\n===========================");
                            Console.WriteLine($"Name: {current.Name}");
                            Console.WriteLine($"Bio: {current.Bio}");
                            Console.WriteLine($"Password: {current.Password}");
                            Console.WriteLine($"Is online: {current.isOnline}");
                            Console.WriteLine($"Is muted: {current.isMuted}");
                            Console.WriteLine($"Is banned: {current.isBanned}");
                            Console.WriteLine($"Is administrator: {current.isAdmin}");
                            Console.WriteLine($"Messages sent: {current.SentMessages}");
                            Console.WriteLine("===========================\n");
                            break;
                        }
                    }
                    if (!exist)
                    {
                        Console.WriteLine($"There's no user named {username}");
                    }
                }
            }
            if (input.StartsWith("/ban "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string username = parts[1];
                    bool exist = false;
                    foreach (User current in users)
                    {
                        if (current.Name == username)
                        {
                            exist = true;
                            if (!current.isBanned)
                            {
                                current.isBanned = true;
                                Console.WriteLine($"{username} was banned from this server.");
                            }
                            else
                            {
                                Console.WriteLine($"{username} is already banned!");
                            }
                        }
                    }
                    if (!exist)
                    {
                        Console.WriteLine($"There's no user named {username}");
                    }
                }
            }
            if (input.StartsWith("/unban "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string username = parts[1];
                    bool exist = false;
                    foreach (User current in users)
                    {
                        if (current.Name == username)
                        {
                            exist = true;
                            if (current.isBanned)
                            {
                                current.isBanned = false;
                                Console.WriteLine($"{username} was unbanned on this server.");
                            }
                            else
                            {
                                Console.WriteLine($"{username} is not banned!");
                            }
                        }
                    }
                    if (!exist)
                    {
                        Console.WriteLine($"There's no user named {username}");
                    }
                }
            }
            if (input.StartsWith("/promote "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string username = parts[1];
                    bool exist = false;
                    foreach (User current in users)
                    {
                        if (current.Name == username)
                        {
                            exist = true;
                            if (!current.isAdmin)
                            {
                                current.isAdmin = true;
                                Console.WriteLine($"Now {username} is administrator.");
                            }
                            else
                            {
                                Console.WriteLine($"{username} is already administrator!");
                            }
                        }
                    }
                    if (!exist)
                    {
                        Console.WriteLine($"There's no user named {username}");
                    }
                }
            }
            if (input.StartsWith("/dismiss "))
            {
                string[] parts = input.Split(' ', 2);
                if (parts.Length == 2)
                {
                    string username = parts[1];
                    bool exist = false;
                    foreach (User current in users)
                    {
                        if (current.Name == username)
                        {
                            exist = true;
                            if (current.isAdmin)
                            {
                                current.isAdmin = false;
                                Console.WriteLine($"Now {username} is not administrator.");
                            }
                            else
                            {
                                Console.WriteLine($"{username} is not administrator yet!");
                            }
                        }
                    }
                    if (!exist)
                    {
                        Console.WriteLine($"There's no user named {username}");
                    }
                }
            }
            if (input == "/mutelist")
            {
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
                if (users.Count > 0)
                {
                    foreach (User current in users)
                    {
                        if (!current.isMuted)
                        {
                            current.isMuted = true;
                        }
                    }
                    Console.WriteLine("\nNow nobody cannot send messages.\n");
                    BroadcastMessage("chat", "Everybody in this conversation was muted by administrator.");
                }
                else
                {
                    Console.WriteLine("\nThere's no users on server yet.\n");
                }
            }
            if (input == "/unmuteall")
            {
                if (users.Count > 0)
                {
                    int old_number_of_muted_users = 0;

                    foreach (User current in users)
                    {
                        if (current.isMuted)
                        {
                            current.isMuted = false;
                            old_number_of_muted_users++;
                        }
                    }

                    if (old_number_of_muted_users > 0)
                    {
                        Console.WriteLine($"\nAll users from mutelist ({old_number_of_muted_users}) was unmuted.\n");
                        BroadcastMessage("chat", $"All users from mutelist ({old_number_of_muted_users}) was unmuted.");
                    }
                    else
                    {
                        Console.WriteLine("There's no muted users yet.");
                    }
                }
                else
                {
                    Console.WriteLine("\nThere's no users on server yet!\n");
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
            if (input == "/users")
            {
                int count = 1;
                string state;
                if (users.Count > 0)
                {
                    foreach (User current in users)
                    {
                        if (current.isOnline)
                        {
                            state = "Online";
                        }
                        else
                        {
                            state = "Offline";
                        }
                        Console.WriteLine($"{count}. {current.Name} ({state})");
                        count++;
                    }
                }
                else
                {
                    Console.WriteLine("No users on server yet...");
                }
            }
            if (input == "/save")
            {
                SaveHistory();
                SaveAccounts();
            }
            if (input == "/read")
            {
                ReadHistory();
                ReadAccounts();
            }
        }
    }

    static void SaveHistory()
    {
        string filePath = @"history.json";
        string content = "{";
        int number_of_msgs = messages.Count;
        int number = 1;
        if (File.Exists(filePath))
        {
            Console.WriteLine("File is ready.");
        }
        else
        {
            File.Create(filePath).Close();
            Console.WriteLine("File created");
        }

        foreach (Message current in messages)
        {
            content += ($"\"{number}\": " + $"\"{current.Json}\"");
            if (number < number_of_msgs)
            {
                content += ", ";
            }
            number++;
        }

        content += "}";
        File.WriteAllText(filePath, content);
    }

    static void ReadHistory()
    {
        string filePath = @"history.json";
        if (File.Exists(filePath))
        {
            int number = 1;
            string readContent = File.ReadAllText(filePath);
            while (true)
            {
                try
                {
                    string res = ParseJson(readContent, $"{number}");
                    string username = ParseJson(res, "from");
                    string text = ParseJson(res, "text");
                    string to = ParseJson(res, "to");
                    string time = ParseJson(res, "time");
                    string escapedMessage = Regex.Replace(res, "\"", "\\\"");
                    messages.Add(new Message(username, to, text, time, escapedMessage));
                }
                catch
                {
                    break;
                }
                number++;
            }
        }
    }

    static void SaveAccounts()
    {
        string filePath = @"accounts.json";
        string content = "{";
        int number_of_accounts = users.Count;
        int number = 1;
        if (File.Exists(filePath))
        {
            Console.WriteLine("File is ready.");
        }
        else
        {
            File.Create(filePath).Close();
            Console.WriteLine("File created");
        }

        foreach (User current in users)
        {
            current.SetJson();
            content += ($"\"{number}\": " + $"\"{current.Json}\"");
            if (number < number_of_accounts)
            {
                content += ", ";
            }
            number++;
        }

        content += "}";

        File.WriteAllText(filePath, content);
    }

    static void ReadAccounts()
    {
        string filePath = @"accounts.json";
        if (File.Exists(filePath))
        {
            int number = 1;
            string readContent = File.ReadAllText(filePath);
            while (true)
            {
                try
                {
                    string res = ParseJson(readContent, $"{number}");
                    string username = ParseJson(res, "name");
                    string password = ParseJson(res, "password");
                    string bio = ParseJson(res, "bio");
                    int sent_msgs = Convert.ToInt32(ParseJson(res, "sent_msgs"));
                    bool isOnline = Convert.ToBoolean(ParseJson(res, "isOnline"));
                    bool isMuted = Convert.ToBoolean(ParseJson(res, "isMuted"));
                    bool isBanned = Convert.ToBoolean(ParseJson(res, "isBanned"));
                    bool isAdmin = Convert.ToBoolean(ParseJson(res, "isAdmin"));
                    string escapedJson = Regex.Replace(res, "\"", "\\\"");
                    users.Add(new User(username, password, bio, sent_msgs, isMuted, isBanned, isAdmin));
                }
                catch
                {
                    break;
                }
                number++;
            }
        }
    }
}