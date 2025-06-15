using Domain;
using System.Net.WebSockets;
using System.Text;

namespace Infrastructure
{
    public class BroadcastMessage : IBroadcastMessage
    {
        public void Broadcast(Message message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message.Json);

            List<WebSocket> webSocketClients = new List<WebSocket>();

            foreach (User u in message.Receiver.Participants)
            {
                foreach (var s in u.NewSessions)
                {
                    webSocketClients.Add(s);
                }
            }

            Send(data, webSocketClients);
        }

        public void DeleteMessage(Message message)
        {
            byte[] data = Encoding.UTF8.GetBytes("{ " + $"\"type\": \"delete_message\", \"dialogue_id\": \"{message.Receiver.Id}\", \"message_id\": \"{message.Id}\"" + " }");

            List<WebSocket> webSocketClients = new List<WebSocket>();

            foreach (User u in message.Receiver.Participants)
            {
                foreach (var s in u.NewSessions)
                {
                    webSocketClients.Add(s);
                }
            }

            Send(data, webSocketClients);
        }

        public void EditMessage(Message message)
        {
            byte[] data = Encoding.UTF8.GetBytes("{ " + $"\"type\": \"edit_message\"," +
                $" \"dialogue_id\": \"{message.Receiver.Id}\", " +
                $"\"message_id\": \"{message.Id}\", " +
                $"\"text\": \"{message.Text}\", " +
                $"\"timestamp\": \"{message.TimeStamp}\"" +
                " }");

            List<WebSocket> webSocketClients = new List<WebSocket>();

            foreach (User u in message.Receiver.Participants)
            {
                foreach (var s in u.NewSessions)
                {
                    webSocketClients.Add(s);
                }
            }

            Send(data, webSocketClients);
        }

        public void CreateDialogue(Dialogue _dialogue)
        {
            byte[] data = Encoding.UTF8.GetBytes("{ " + $"\"type\": \"add_dialogue\"," +
                $" \"dialogue_id\": \"{_dialogue.Id}\", " +
                $"\"dialogue_name\": \"{_dialogue.Name}\", " +
                $"\"dialogue_type\": \"{_dialogue.Type}\", " +
                $"\"avatar\": \"{_dialogue.AvatarUrl}\"" +
                " }");

            List<WebSocket> webSocketClients = _dialogue.Creator.NewSessions;

            Send(data, webSocketClients);
        }

        public void UpdateDialogue(Dialogue _dialogue)
        {
            byte[] data = Encoding.UTF8.GetBytes("{ " + $"\"type\": \"edit_dialogue\"," +
                $" \"dialogue_id\": \"{_dialogue.Id}\", " +
                $"\"dialogue_name\": \"{_dialogue.Name}\", " +
                $"\"dialogue_type\": \"{_dialogue.Type}\", " +
                $"\"avatar\": \"{_dialogue.AvatarUrl}\"" +
                " }");

            List<WebSocket> webSocketClients = new List<WebSocket>();

            foreach (User u in _dialogue.Participants)
            {
                foreach (var s in u.NewSessions)
                {
                    webSocketClients.Add(s);
                }
            }

            Send(data, webSocketClients);
        }

        public void DeleteDialogue(Dialogue _dialogue)
        {
            byte[] data = Encoding.UTF8.GetBytes("{ " + $"\"type\": \"delete_dialogue\"," +
                $" \"dialogue_id\": \"{_dialogue.Id}\", " +
                $"\"dialogue_type\": \"{_dialogue.Type}\"" +
                " }");

            List<WebSocket> webSocketClients = new List<WebSocket>();

            foreach (User u in _dialogue.Participants)
            {
                foreach (var s in u.NewSessions)
                {
                    webSocketClients.Add(s);
                }
            }

            Send(data, webSocketClients);
        }

        public void CreateUser(User _user, List<User> users)
        {
            byte[] data = Encoding.UTF8.GetBytes("{ " + $"\"type\": \"create_user\"," +
                $" \"user_id\": \"{_user.Id}\", " +
                $"\"name\": \"{_user.Name}\", " +
                $"\"avatar\": \"{_user.AvatarUrl}\"" +
                " }");

            List<WebSocket> webSocketClients = new List<WebSocket>();

            foreach (User u in users)
            {
                foreach (var s in u.NewSessions)
                {
                    webSocketClients.Add(s);
                }
            }

            Send(data, webSocketClients);
        }

        public void UpdateUser(User _user, List<User> users)
        {
            byte[] data = Encoding.UTF8.GetBytes("{ " + $"\"type\": \"edit_user\"," +
                $" \"user_id\": \"{_user.Id}\", " +
                $"\"name\": \"{_user.Name}\", " +
                $"\"avatar\": \"{_user.AvatarUrl}\"" +
                " }");

            List<WebSocket> webSocketClients = new List<WebSocket>();

            foreach (User u in users)
            {
                foreach (var s in u.NewSessions)
                {
                    webSocketClients.Add(s);
                }
            }

            Send(data, webSocketClients);
        }

        public void DeleteUser(User _user, List<User> users)
        {
            byte[] data = Encoding.UTF8.GetBytes("{ " + $"\"type\": \"delete_user\"," +
                $" \"user_id\": \"{_user.Id}\"" +
                " }");

            List<WebSocket> webSocketClients = new List<WebSocket>();

            foreach (User u in users)
            {
                foreach (var s in u.NewSessions)
                {
                    webSocketClients.Add(s);
                }
            }

            Send(data, webSocketClients);
        }

        public void Send(byte[] data, List<WebSocket> webSocketClients)
        {
            lock (webSocketClients)
            {
                foreach (var client in webSocketClients)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            client.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("\nError broadcasting to client: " + ex.Message);
                        }
                    }
                }
            }
        }
    }
}
