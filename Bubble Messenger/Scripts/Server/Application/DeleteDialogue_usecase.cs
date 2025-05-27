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
                Repo.DeleteDialogue(id);
                return Result.SUCCESS;
            }
            else
            {
                return Result.DIALOGUE_DOESNT_EXIST;
            }
        }
    }
}
