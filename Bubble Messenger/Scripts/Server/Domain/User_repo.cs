namespace Domain
{
    public interface IUserRepo
    {
        public User GetUserById(string id);

        public User GetUserByName(string name);

        public void CreateUser(string name, string password);

        public void DeleteUser(string id);

        public void UpdateUser(User user);
    }
}