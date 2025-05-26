using Domain;

namespace Application
{
    public class CreateDialogueUsecase
    {
        IDialogueRepo DialogueRepo { get; }

        public enum Result
        {
            SUCCESS,
            DIALOGUE_NAME_IS_NOT_UNIQUE
        }

        public CreateDialogueUsecase(IDialogueRepo dialogueRepo)
        {
            DialogueRepo = dialogueRepo;
        }

        public Result Execute(string name, string type)
        {
            if (DialogueRepo.GetDialogueByName(name) != null)
            {
                return Result.DIALOGUE_NAME_IS_NOT_UNIQUE;
            }
            else
            {
                DialogueRepo.CreateDialogue(name, type);

                Console.WriteLine($"\nNew dialogue created:\nName: {name}\nType: {type}");

                return Result.SUCCESS;
            }
        }
    }
}
