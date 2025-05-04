using System.Text.RegularExpressions;

namespace Domain
{
    public class User
    {
        public string Id { get; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Bio { get; set; }

        public bool isOnline { get; set; }

        public string Json { get; set; }

        public User(IIdProvider provider, string name, string password, string bio)
        {
            Id = provider.GetId(20);
            Name = name;
            Password = password;
            Bio = bio;
            isOnline = false;
            SetJson();
        }

        public void SetJson()
        {
            string json = "{" +
                $"\"name\": \"{Name}\"," +
                $"\"id\": \"{Id}\"," +
                $"\"password\": \"{Password}\"," +
                $"\"bio\": \"{Bio}\"" +
                "}";
            Json = Regex.Replace(json, "\"", "\\\"");
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

        public Message(IIdProvider provider, User sender, Dialogue receiver, string text, string time, string json)
        {
            Sender = sender;
            Receiver = receiver;
            Text = text;
            TimeStamp = time;
            Json = json;
            Seen = false;
            Id = provider.GetId(20);
        }
    }

    public class Dialogue
    {
        public List<User> Participants { get; set; }

        public List<User> Administrators { get; set; }

        public List<User> Muted { get; set; }

        public List<User> Banned { get; set; }

        public List<Message> Messages { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public Dialogue(IIdProvider provider, string name, string type)
        {
            Participants = new List<User>();
            Administrators = new List<User>();
            Muted = new List<User>();
            Banned = new List<User>();
            Messages = new List<Message>();
            Name = name;
            Type = type;
            Id = provider.GetId(20);
        }
    }
}