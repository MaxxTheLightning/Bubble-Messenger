using Domain;

namespace Application
{
    public class DeleteUserUsecase
    {
        IUserRepo Repo { get; }

        IBroadcastMessage BroadcastMessage { get; }

        public enum Result
        {
            SUCCESS,
            USER_DOESNT_EXIST,
            INVALID_PASSWORD
        }

        public DeleteUserUsecase(IUserRepo repo, IBroadcastMessage broadcastMessage)
        {
            Repo = repo;
            BroadcastMessage = broadcastMessage;
        }

        public Result Execute(string id, string password)
        {
            User _user = Repo.GetUserById(id);

            if (_user != null)
            {
                if (_user.Password == password)
                {
                    string _deadUsername = _user.Name;

                    BroadcastMessage.DeleteUser(_user, Repo.GetAllUsers());

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
