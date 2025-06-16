using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace Domain
{
    public class User
    {
        public string Id { get; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Bio { get; set; }

        public string Color { get; set; }

        public bool isOnline { get; set; }

        public string AvatarUrl { get; set; }

        public string Json { get; set; }

        public List<WebSocket> NewSessions { get; set; }

        public User(IIdProvider provider, string name, string password)
        {
            Id = provider.GetId(20);
            Name = name;
            Password = password;
            Bio = "No bio...";
            Color = "#ffffff";
            AvatarUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSDoKp0wum3Z8G1cQXa7j9UtFbpTYqG5YhUcg&s";
            isOnline = false;
            NewSessions = new List<WebSocket>();
            Json = "{" +
                $"\"name\": \"{Name}\"," +
                $"\"id\": \"{Id}\"," +
                $"\"password\": \"{Password}\"," +
                $"\"bio\": \"{Bio}\"" +
                "}";

            Console.WriteLine($"\nNew account created.\nName: {Name}\nPassword: {Password}\nID: {Id}");
        }
    }

    public class Message
    {
        public string Id { get; }

        public User Sender { get; set; }

        public string Text { get; set; }

        public Dialogue Receiver { get; }

        public string TimeStamp { get; set; }

        public string Json { get; set; }

        public bool Seen { get; set; }

        public bool Edited { get; set; }

        public Message(IIdProvider provider, User sender, Dialogue receiver, string text, string time)
        {
            Sender = sender;
            Receiver = receiver;
            Text = text;
            TimeStamp = time;
            Seen = false;
            Edited = false;
            Id = provider.GetId(20);
            SetJson();

            Console.WriteLine($"\nNew message sent.\nSender: {Sender.Name}\nReceiver: {Receiver.Name} ({Receiver.Type})\nText: {Text}\nTime: {TimeStamp}\nID: {Id}");
        }

        public void SetJson()
        {
            string json = "{" +
                "\"type\": \"send\"," +
                $" \"id\" : \"{Id}\"," +
                $" \"sender_name\" : \"{Sender.Name}\"," +
                $" \"sender_id\" : \"{Sender.Id}\"," +
                $" \"receiver_id\" : \"{Receiver.Id}\"," +
                $" \"text\" : \"{Text}\"," +
                $" \"timestamp\" : \"{TimeStamp}\"," +
                $" \"color\" : \"{Sender.Color}\"" +
                "}";
            Json = json;
        }
    }

    public class Dialogue
    {
        public List<User> Participants { get; set; }

        public List<User> Administrators { get; set; }

        public List<User> Muted { get; set; }

        public List<User> Banned { get; set; }

        public List<Message> Messages { get; set; }

        public User Creator { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string AvatarUrl { get; set; }

        public string Bio { get; set; }

        public Dialogue(IIdProvider provider, string name, string type, User creator)
        {
            Participants = new List<User>();
            Participants.Add(creator);
            Administrators = new List<User>();
            Administrators.Add(creator);
            Muted = new List<User>();
            Banned = new List<User>();
            Messages = new List<Message>();
            Name = name;
            Type = type;
            Creator = creator;
            AvatarUrl = "";
            Id = provider.GetId(20);

            Console.WriteLine($"\nNew dialogue created.\nName: {Name}\nCreator: {Creator.Name}\nType: {Type}\nID: {Id}");
        }
    }
}