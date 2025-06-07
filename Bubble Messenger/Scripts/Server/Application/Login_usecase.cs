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
            User _user = Repo.GetUserByName(name);  //  Пробуем получить пользователя

            if (_user != null)
            {
                if (_user.Password == password)
                {
                    Console.WriteLine($"\nUser {_user.Name} loginned successfully.");

                    return Result.SUCCESS;
                }
                else
                {
                    Console.WriteLine($"\nUser {_user.Name} tried to login, but password is incorrect.\nEntered password: {password}\nCorrect password: {_user.Password}");

                    return Result.INVALID_PASSWORD;
                }
            }
            else
            {
                Console.WriteLine($"\nAccount \"{name}\" doesn't exist. (Login attempt)");

                return Result.USER_DOESNT_EXIST;
            }
        }
    }
}
