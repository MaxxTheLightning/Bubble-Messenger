using System.Net.WebSockets;

namespace Domain
{
    public interface IBroadcastMessage
    {
        public void Broadcast(Message message);

        public void DeleteMessage(Message message);

        public void EditMessage(Message message);

        public void CreateDialogue(Dialogue dialogue);

        public void UpdateDialogue(Dialogue dialogue);

        public void DeleteDialogue(Dialogue dialogue);

        public void CreateUser(User user);

        public void UpdateUser(User user);

        public void DeleteUser(User user);
    }
}
