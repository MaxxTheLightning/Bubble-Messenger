using Domain;

namespace Application
{
    public class LoginUsecase
    {
        IUserRepo Repo { get; }
        

        public enum Result
        {
            SUCCESS,
            USER_DOESNT_EXIST,
            INVALID_PASSWORD
        }

        public LoginUsecase(IUserRepo repo)
        {
            Repo = repo;
        }

        public Result Execute(string name, string password)
        {
            User _user = Repo.GetUserByName(name);

            if (_user != null)
            {
                if (_user.Password == password)
                {
                    return Result.SUCCESS;
                }
                else
                {
                    return Result.INVALID_PASSWORD;
                }
            }
            else
            {
                return Result.USER_DOESNT_EXIST;
            }
        }
    }
}
