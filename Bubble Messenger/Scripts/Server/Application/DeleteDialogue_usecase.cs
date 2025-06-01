using Domain;

namespace Application
{
    public class DeleteDialogueUsecase
    {
        IDialogueRepo Repo { get; }

        public enum Result
        {
            SUCCESS,
            DIALOGUE_DOESNT_EXIST
        }

        public DeleteDialogueUsecase(IDialogueRepo repo)
        {
            Repo = repo;
        }

        public Result Execute(string id)
        {
            Dialogue _dialogue = Repo.GetDialogueById(id);

            if (_dialogue != null)
            {
                string _deadDialogueName = _dialogue.Name;

                Repo.DeleteDialogue(id);

                Console.WriteLine($"\nDialogue {_deadDialogueName} deleted successfully.");

                return Result.SUCCESS;
            }
            else
            {
                Console.WriteLine($"\nDialogue id={id} doesn't exist. (Deleting attempt)");

                return Result.DIALOGUE_DOESNT_EXIST;
            }
        }
    }
}
