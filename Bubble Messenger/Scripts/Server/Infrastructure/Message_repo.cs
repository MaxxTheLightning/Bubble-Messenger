using Domain;

namespace Infrastructure
{
    public class MockMessageRepo : IMessageRepo
    {
        public List<Message> Messages { get; set; }

        IIdProvider IdProvider { get; }

        public MockMessageRepo(IIdProvider idProvider)
        {
            Messages = new List<Message>();
            IdProvider = idProvider;
        }

        public Message GetMessage(string id)
        {
            foreach (Message message in Messages)
            {
                if (message.Id == id)
                {
                    return message;
                }
            }

            return null;
        }

        public void CreateMessage(User sender, Dialogue receiver, string text, string time, string timestamp, string json)
        {
            Message new_message = new Message(IdProvider, sender, receiver, text, time, json);
            Messages.Add(new_message);
        }

        public void DeleteMessage(string id)
        {
            foreach (Message message in Messages)
            {
                if (message.Id == id)
                {
                    Messages.Remove(message);
                }
            }
        }

        public void UpdateMessage(Message message)
        {
            foreach (Message msg in Messages)
            {
                if (msg.Id == message.Id)
                {
                    Messages[Messages.IndexOf(msg)] = message;
                }
            }
        }
    }
}