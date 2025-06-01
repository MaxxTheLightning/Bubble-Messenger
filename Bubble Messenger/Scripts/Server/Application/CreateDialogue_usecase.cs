using Domain;

namespace Application
{
    public class CreateDialogueUsecase
    {
        IDialogueRepo DialogueRepo { get; }

        IUserRepo UserRepo { get; }

        public enum Result
        {
            SUCCESS
        }

        public CreateDialogueUsecase(IDialogueRepo dialogueRepo, IUserRepo userRepo)
        {
            DialogueRepo = dialogueRepo;
            UserRepo = userRepo;
        }

        public Result Execute(string name, string type, string creator_name)
        {
            DialogueRepo.CreateDialogue(name, type, UserRepo.GetUserByName(creator_name));

            return Result.SUCCESS;
        }
    }
}
