namespace Domain
{
    public interface IMessageRepo
    {
        public Message GetMessage(string id);

        public void CreateMessage(User sender, Dialogue receiver, string text, string time, string timestamp, string json);

        public void DeleteMessage(string id);

        public void UpdateMessage(Message message);
    }
}