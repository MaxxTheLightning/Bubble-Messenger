using Domain;

namespace Application
{
    public class DeleteDialogueUsecase
    {
        IDialogueRepo Repo { get; }

        IUserRepo UserRepo { get; }

        public enum Result
        {
            SUCCESS,
            DIALOGUE_DOESNT_EXIST,
            EMPTY_CREATOR,
            INVALID_PASSWORD
        }

        public DeleteDialogueUsecase(IDialogueRepo repo, IUserRepo userRepo)
        {
            Repo = repo;
            UserRepo = userRepo;
        }

        public Result Execute(string dialogue_id, string creator_id, string creator_password)
        {
            Dialogue _dialogue = Repo.GetDialogueById(dialogue_id);

            if (UserRepo.GetUserById(creator_id) == null)
            {
                Console.WriteLine($"\nError deleting dialogue: empty creator.");

                return Result.EMPTY_CREATOR;
            }
            else if (UserRepo.GetUserById(creator_id).Password != creator_password)
            {
                Console.WriteLine("\nPasswords don't match. (Deleting message attempt)");

                return Result.INVALID_PASSWORD;
            }
            else if (_dialogue != null)
            {
                string _deadDialogueName = _dialogue.Name;

                Repo.DeleteDialogue(dialogue_id);

                Console.WriteLine($"\nDialogue {_deadDialogueName} deleted successfully.");

                return Result.SUCCESS;
            }
            else
            {
                Console.WriteLine($"\nDialogue id={dialogue_id} doesn't exist. (Deleting attempt)");

                return Result.DIALOGUE_DOESNT_EXIST;
            }
        }
    }
}
