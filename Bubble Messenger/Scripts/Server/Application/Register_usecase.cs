using Domain;

namespace Application
{
    public class RegisterUsecase
    {
        IUserRepo Repo { get; }

        public enum Result
        {
            SUCCESS,
            USER_ALREADY_EXISTS
        }

        public RegisterUsecase(IUserRepo repo)
        {
            Repo = repo;
        }

        public Result Execute(string name, string password, string bio)
        {
            if (Repo.GetUserByName(name) == null)
            {
                Repo.CreateUser(name, password, bio);

                return Result.SUCCESS;
            }
            else
            {
                return Result.USER_ALREADY_EXISTS;
            }
        }
    }
}
