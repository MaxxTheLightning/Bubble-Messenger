using Domain;

namespace Application
{
    public class CreateDialogueUsecase
    {
        IDialogueRepo DialogueRepo { get; }

        IUserRepo UserRepo { get; }

        public enum Result
        {
            SUCCESS,
            EMPTY_CREATOR,
            INVALID_PASSWORD
        }

        public CreateDialogueUsecase(IDialogueRepo dialogueRepo, IUserRepo userRepo)
        {
            DialogueRepo = dialogueRepo;
            UserRepo = userRepo;
        }

        public Result Execute(string dialogue_name, string dialogue_type, string creator_id, string creator_password)
        {
            if (UserRepo.GetUserById(creator_id) == null)
            {
                Console.WriteLine($"\nError creating dialogue: empty creator.");

                return Result.EMPTY_CREATOR;
            }
            else if (UserRepo.GetUserById(creator_id).Password != creator_password)
            {
                Console.WriteLine("\nPasswords don't match. (Deleting message attempt)");

                return Result.INVALID_PASSWORD;
            }
            else
            {
                DialogueRepo.CreateDialogue(dialogue_name, dialogue_type, UserRepo.GetUserById(creator_id));

                return Result.SUCCESS;
            }
        }
    }
}
