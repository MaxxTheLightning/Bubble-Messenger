namespace Domain
{
    public interface IUserRepo
    {
        public User GetUserById(string id);

        public User GetUserByName(string name);

        public void CreateUser(string name, string password, string bio);

        public void DeleteUser(string name);

        public void UpdateUser(User user);
    }
}