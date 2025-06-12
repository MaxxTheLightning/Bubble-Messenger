using Domain;

namespace Application
{
    public class UpdateDialogueUsecase
    {
        IDialogueRepo Repo { get; }

        IUserRepo UserRepo { get; }

        public enum Result
        {
            SUCCESS,
            SAME_NAME,
            EMPTY_NAME,
            EMPTY_PARTICIPANT,
            DIALOGUE_NOT_FOUND,
            USER_NOT_FOUND,
            NO_PARTICIPANT,
            ALREADY_PARTICIPANT,
            ALREADY_MUTED,
            ALREADY_BANNED,
            ALREADY_ADMIN,
            NOT_PARTICIPANT,
            AUTHOR_NOT_FOUND,
            INVALID_PASSWORD,
            ERROR
        }

        public UpdateDialogueUsecase(IDialogueRepo repo, IUserRepo user_repo)
        {
            Repo = repo;
            UserRepo = user_repo;
        }

        public Result Execute(string id, string author_id, string author_password, string new_name, string about, string avatarUrl, string participants_IDs, string admins_IDs, string banned_IDs, string muted_IDs)
        {
            string[] participants_IDs_array = participants_IDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] admins_IDs_array = admins_IDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] banned_IDs_array = banned_IDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] muted_IDs_array = muted_IDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (UserRepo.GetUserById(author_id) == null)
            {
                Console.WriteLine($"Author {author_id} not found. (Dialogue editing attempt)");

                return Result.AUTHOR_NOT_FOUND;
            }
            else if (UserRepo.GetUserById(author_id).Password != author_password)
            {
                Console.WriteLine($"\nPasswors not match. (Dialogue editing attempt)");

                return Result.INVALID_PASSWORD;
            }
            else if (Repo.GetDialogueById(id) == null)
            {
                Console.WriteLine($"\nDialogue id={id} not found. (Rename attempt)");

                return Result.DIALOGUE_NOT_FOUND;
            }
            else if (Repo.GetDialogueById(id).Name == new_name)
            {
                Console.WriteLine($"\nDialogue id={id} already has the name \"{new_name}\". (Rename attempt)");

                return Result.SAME_NAME;
            }
            else if (new_name == "")
            {
                Console.WriteLine($"\nDialogue id={id} cannot have the empty name. (Rename attempt)");

                return Result.EMPTY_NAME;
            }
            else if (participants_IDs_array != null)
            {
                Dialogue _target = Repo.GetDialogueById(id);
                string _deadDialogueName = _target.Name;

                _target.Name = new_name;

                List<User> _participants = new List<User>();
                List<User> _admins = new List<User>();
                List<User> _banned = new List<User>();
                List<User> _muted = new List<User>();

                foreach (string uid in participants_IDs_array)
                {
                    User user = UserRepo.GetUserById(uid);
                    _participants.Add(user);
                }

                foreach (string uid in admins_IDs_array)
                {
                    User user = UserRepo.GetUserById(uid);
                    _admins.Add(user);
                }

                foreach (string uid in banned_IDs_array)
                {
                    User user = UserRepo.GetUserById(uid);
                    _banned.Add(user);
                }

                foreach (string uid in muted_IDs_array)
                {
                    User user = UserRepo.GetUserById(uid);
                    _muted.Add(user);
                }

                _target.Participants = _participants;
                _target.Administrators = _admins;
                _target.Banned = _banned;
                _target.Muted = _muted;

                _target.Bio = about;

                _target.AvatarUrl = avatarUrl;

                Repo.UpdateDialogue(_target);

                Console.WriteLine($"\nDialogue {_deadDialogueName} updated successfully.\nNew name: {new_name}\nNew bio: {about}\nNew avatar: {avatarUrl}");
                Console.WriteLine("Participants:");
                foreach (string uid in participants_IDs_array)
                {
                    User user = UserRepo.GetUserById(uid);
                    Console.WriteLine(user.Name);
                }
                Console.WriteLine("Admins:");
                foreach (string uid in admins_IDs_array)
                {
                    User user = UserRepo.GetUserById(uid);
                    Console.WriteLine(user.Name);
                }
                Console.WriteLine("Banned:");
                foreach (string uid in banned_IDs_array)
                {
                    User user = UserRepo.GetUserById(uid);
                    Console.WriteLine(user.Name);
                }
                Console.WriteLine("Muted:");
                foreach (string uid in muted_IDs_array)
                {
                    User user = UserRepo.GetUserById(uid);
                    Console.WriteLine(user.Name);
                }


                return Result.SUCCESS;
            }
            else
            {
                return Result.ERROR;
            }
        }
    }
}
