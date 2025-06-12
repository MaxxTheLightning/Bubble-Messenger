using Domain;

namespace Application
{
    public class ViewDialogueUsecase
    {
        IDialogueRepo DialogueRepo { get; }

        public enum Result
        {
            DIALOGUE_NOT_FOUND,
            SUCCESS
        }

        public ViewDialogueUsecase(IDialogueRepo dialogueRepo)
        {
            DialogueRepo = dialogueRepo;
        }

        public Result Execute(string id)
        {
            Dialogue _dialogue = DialogueRepo.GetDialogueById(id);

            if (_dialogue != null)
            {
                return Result.SUCCESS;
            }
            else
            {
                Console.WriteLine($"\nDialogue {id} doesn't exist. (View dialogue attempt)");

                return Result.DIALOGUE_NOT_FOUND;
            }
        }
    }
}
