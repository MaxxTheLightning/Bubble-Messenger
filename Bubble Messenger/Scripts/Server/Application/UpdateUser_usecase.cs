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

        public Result Execute(string id, string password, string new_name, string new_password, string new_bio, string new_color, string new_avatarUrl)
        {
            if (Repo.GetUserById(id) == null)
            {
                Console.WriteLine($"\nUser {id} doesn't exist. (Updating attempt)");

                return Result.NOT_FOUND;
            }
            else if (Repo.GetUserById(id) != null && (Repo.GetUserByName(new_name) != null))
            {
                Console.WriteLine($"\nUser with name \"{new_name}\" already exists. (Updating attempt)");

                return Result.NOT_UNIQUE_NAME;
            }
            else
            {
                User _target = Repo.GetUserById(id);

                string _deadname = _target.Name;

                _target.Name = new_name;
                _target.Password = new_password;
                _target.Bio = new_bio;
                _target.Color = new_color;
                _target.AvatarUrl = new_avatarUrl;

                Repo.UpdateUser(_target);

                Console.WriteLine($"\nUser {_deadname} updated successfully.\nID: {_target.Id}\nName: {new_name}\nPassword: {password}\nBio: {new_bio}\nColor: {new_color}\nAvatar URL: {new_avatarUrl}");

                return Result.SUCCESS;
            }
        }
    }
}
