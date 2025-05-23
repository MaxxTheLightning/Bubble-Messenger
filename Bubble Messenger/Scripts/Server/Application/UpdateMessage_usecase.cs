using Domain;

namespace Application
{
    public class UpdateMessageUsecase
    {
        IMessageRepo Repo { get; }

        public enum Result
        {
            SUCCESS,
            SAME,
            EMPTY_MESSAGE,
            NOT_FOUND
        }

        public UpdateMessageUsecase(IMessageRepo repo)
        {
            Repo = repo;
        }

        public Result Execute(string id, string new_text)
        {
            if (Repo.GetMessage(id) == null)
            {
                return Result.NOT_FOUND;
            }
            else if (Repo.GetMessage(id).Text == new_text)
            {
                return Result.SAME;
            }
            else if (new_text == "")
            {
                return Result.EMPTY_MESSAGE;
            }
            else
            {
                Message _target = Repo.GetMessage(id);
                _target.Text = new_text;

                Repo.UpdateMessage(_target);

                return Result.SUCCESS;
            }
        }
    }
}
