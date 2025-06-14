using Domain;

namespace Infrastructure
{
    public class MockUserRepo : IUserRepo
    {
        public List<User> Users { get; set; }

        IIdProvider IdProvider { get; }

        IBroadcastMessage BroadcastMessaage { get; }

        public MockUserRepo(IIdProvider idProvider, IBroadcastMessage broadcastMessaage)
        {
            Users = new List<User>();
            IdProvider = idProvider;
            BroadcastMessaage = broadcastMessaage;
        }

        public List<User> GetAllUsers()
        {
            return Users;
        }

        public User GetUserById(string id)
        {
            foreach (User user in Users.ToList())
            {
                if (user.Id == id)
                {
                    return user;
                }
            }

            return null;
        }

        public User GetUserByName(string name)
        {
            foreach (User user in Users.ToList())
            {
                if (user.Name == name)
                {
                    return user;
                }
            }

            return null;
        }

        public void CreateUser(string name, string password)
        {
            User new_user = new User(IdProvider, name, password);
            Users.Add(new_user);
            BroadcastMessaage.CreateUser(new_user);
        }

        public void DeleteUser(string id)
        {
            foreach (User user in Users.ToList())
            {
                if (user.Id == id)
                {
                    Users.Remove(user);
                    BroadcastMessaage.DeleteUser(user);
                    break;
                }
            }
        }

        public void UpdateUser(User user)
        {
            foreach (User u in Users.ToList())
            {
                if (u.Id == user.Id)
                {
                    Users[Users.IndexOf(u)] = user;
                    BroadcastMessaage.UpdateUser(user);
                    break;
                }
            }
        }
    }
}