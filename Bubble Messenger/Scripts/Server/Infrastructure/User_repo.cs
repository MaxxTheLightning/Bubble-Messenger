using Domain;

namespace Infrastructure
{
    class MockUserRepo : IUserRepo
    {
        public List<User> Users { get; set; }

        public MockUserRepo()
        {
            Users = new List<User>();
        }

        public User GetUser(string id)
        {
            foreach(User user in Users)
            {
                if (user.Id == id)
                {
                    return user;
                }
            }

            return null;
        }

        public void CreateUser(string name, string password, string bio)
        {
            var provider = new RandomIdProvider();
            User new_user = new User(provider, name, password, bio);
            Users.Add(new_user);
        }

        public void DeleteUser(string id)
        {
            foreach (User user in Users)
            {
                if (user.Id == id)
                {
                    Users.Remove(user);
                }
            }
        }

        public void UpdateUser(User user)
        {

        }
    }
}