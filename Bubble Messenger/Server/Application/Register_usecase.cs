using Domain;

namespace Application
{
    public class RegisterUsecase
    {
        IUserRepo Repo { get; }

        IBroadcastMessage BroadcastMessage { get; }

        public enum Result
        {
            SUCCESS,
            USER_ALREADY_EXISTS
        }

        public RegisterUsecase(IUserRepo repo, IBroadcastMessage broadcastMessage)
        {
            Repo = repo;
            BroadcastMessage = broadcastMessage;
        }

        public Result Execute(string name, string password)
        {
            if (Repo.GetUserByName(name) == null)
            {
                Repo.CreateUser(name, password);

                BroadcastMessage.CreateUser(Repo.GetUserByName(name), Repo.GetAllUsers());

                return Result.SUCCESS;
            }
            else
            {
                return Result.USER_ALREADY_EXISTS;
            }
        }
    }
}
