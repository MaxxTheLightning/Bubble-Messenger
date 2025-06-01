using Domain;

namespace Application
{
    public class UpdateUserUsecase
    {
        IUserRepo Repo { get; }

        public enum Result
        {
            SUCCESS,
            NOT_UNIQUE_NAME,
            NOT_FOUND
        }

        public UpdateUserUsecase(IUserRepo repo)
        {
            Repo = repo;
        }

        public Result Execute(string old_name, string new_name, string password, string bio, string color)
        {
            if (Repo.GetUserByName(old_name) == null)
            {
                Console.WriteLine($"\nUser {old_name} doesn't exist. (Updating attempt)");

                return Result.NOT_FOUND;
            }
            else if (Repo.GetUserByName(new_name) != null && (old_name != new_name))
            {
                Console.WriteLine($"\nUser with name \"{new_name}\" already exists. (Updating attempt)");

                return Result.NOT_UNIQUE_NAME;
            }
            else
            {
                User _target = Repo.GetUserByName(old_name);
                _target.Name = new_name;
                _target.Password = password;
                _target.Bio = bio;
                _target.Color = color;

                Repo.UpdateUser(_target);

                Console.WriteLine($"\nUser {old_name} updated successfully.\nID: {_target.Id}\nName: {new_name}\nPassword: {password}\nBio: {bio}\nColor: {color}");

                return Result.SUCCESS;
            }
        }
    }
}
