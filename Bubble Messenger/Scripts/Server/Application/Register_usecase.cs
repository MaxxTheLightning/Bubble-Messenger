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

        public Result Execute(string name, string password)
        {
            if (Repo.GetUserByName(name) == null)
            {
                Repo.CreateUser(name, password);

                return Result.SUCCESS;
            }
            else
            {
                //  Мы попробовали получить пользователя. Если он не null, то значит такой пользователь уже существует.
                return Result.USER_ALREADY_EXISTS;
            }
        }
    }
}
