using Domain;

namespace Application
{
    public class DeleteUserUsecase
    {
        IUserRepo Repo { get; }

        public enum Result
        {
            SUCCESS,
            USER_DOESNT_EXIST,
            INVALID_PASSWORD
        }

        public DeleteUserUsecase(IUserRepo repo)
        {
            Repo = repo;
        }

        public Result Execute(string id, string password)
        {
            User _user = Repo.GetUserById(id);  // Пробуем получить пользователя

            if (_user != null)
            {
                if (_user.Password == password)
                {
                    string _deadUsername = _user.Name;

                    Repo.DeleteUser(id);

                    Console.WriteLine($"\nUser {_deadUsername} deleted successfully.");

                    return Result.SUCCESS;
                }
                else
                {
                    Console.WriteLine($"\nInvalid password of {_user.Name}. (Deleting attempt)\nEntered password: {password}\nCorrect password: {_user.Password}");

                    return Result.INVALID_PASSWORD;
                }
            }
            else
            {
                Console.WriteLine($"\nUser {id} doesn't exist. (Deleting attempt)");

                return Result.USER_DOESNT_EXIST;
            }
        }
    }
}
