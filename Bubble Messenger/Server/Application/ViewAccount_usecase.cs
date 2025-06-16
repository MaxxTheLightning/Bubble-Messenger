using Domain;

namespace Application
{
    public class ViewAccountUsecase
    {
        IUserRepo UserRepo { get; }

        public enum Result
        {
            USER_NOT_FOUND,
            SUCCESS
        }

        public ViewAccountUsecase(IUserRepo userRepo)
        {
            UserRepo = userRepo;
        }

        public Result Execute(string id)
        {
            User _user = UserRepo.GetUserById(id);

            if (_user != null)
            {
                return Result.SUCCESS;
            }
            else
            {
                Console.WriteLine($"\nUser {id} doesn't exist. (View account attempt)");

                return Result.USER_NOT_FOUND;
            }
        }
    }
}
