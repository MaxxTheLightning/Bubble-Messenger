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
                Console.WriteLine($"\nMessage id={id} doesn't exist. (Updating attempt)");

                return Result.NOT_FOUND;
            }
            else if (Repo.GetMessage(id).Text == new_text)
            {
                Console.WriteLine($"\nMessage id={id} has the same text. (Updating attempt)");

                return Result.SAME;
            }
            else if (new_text == "")
            {
                Console.WriteLine($"\nMessage id={id} cannot have the empty text. (Updating attempt)");

                return Result.EMPTY_MESSAGE;
            }
            else
            {
                Message _target = Repo.GetMessage(id);
                _target.Text = new_text;

                Repo.UpdateMessage(_target);

                Console.WriteLine($"\nMessage id={id} updated successfully.\nNew text: \"{new_text}\"");

                return Result.SUCCESS;
            }
        }
    }
}
