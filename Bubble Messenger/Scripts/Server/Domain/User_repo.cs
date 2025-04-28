namespace Domain
{
    interface IUserRepo
    {
        public User GetUser(string id);

        public void CreateUser(string name, string password, string bio);

        public void DeleteUser(string id);

        public void UpdateUser(User user);
    }
}