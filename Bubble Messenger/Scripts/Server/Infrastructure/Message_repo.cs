using Domain;

namespace Infrastructure
{
    public class MockMessageRepo : IMessageRepo
    {
        public List<Message> Messages { get; set; }

        IIdProvider IdProvider { get; }

        IBroadcastMessage BroadcastMessage { get; }

        public MockMessageRepo(IIdProvider idProvider, IBroadcastMessage broadcastMessage)
        {
            Messages = new List<Message>();
            IdProvider = idProvider;
            BroadcastMessage = broadcastMessage;
        }

        public Message GetMessage(string id)
        {
            foreach (Message message in Messages.ToList())
            {
                if (message.Id == id)
                {
                    return message;
                }
            }

            return null;
        }

        public void CreateMessage(User sender, Dialogue receiver, string text, string time)
        {
            Message new_message = new Message(IdProvider, sender, receiver, text, time);
            Messages.Add(new_message);
            BroadcastMessage.Broadcast(new_message);
        }

        public void DeleteMessage(string id)
        {
            foreach (Message message in Messages.ToList())
            {
                if (message.Id == id)
                {
                    Messages.Remove(message);
                }
            }
        }

        public void UpdateMessage(Message message)
        {
            foreach (Message msg in Messages.ToList())
            {
                if (msg.Id == message.Id)
                {
                    Messages[Messages.IndexOf(msg)] = message;
                }
            }
        }
    }
}