using Domain;

namespace Application
{
    public class GetAccountUsecase
    {
        IUserRepo UserRepo { get; }

        public enum Result
        {
            USER_NOT_FOUND,
            INVALID_PASSWORD,
            SUCCESS
        }

        public GetAccountUsecase(IUserRepo userRepo)
        {
            UserRepo = userRepo;
        }

        public Result Execute(string id, string password)
        {
            User _user = UserRepo.GetUserById(id);

            if (_user != null)
            {
                if (_user.Password == password)
                {
                    return Result.SUCCESS;
                }
                else
                {
                    Console.WriteLine($"\nInvalid password of {_user.Name}. (Get account attempt)\nEntered password: {password}\nCorrect password: {_user.Password}");

                    return Result.INVALID_PASSWORD;
                }
            }
            else
            {
                Console.WriteLine($"\nUser {id} doesn't exist. (Get account attempt)");

                return Result.USER_NOT_FOUND;
            }
        }
    }
}
