using Domain;

namespace Application
{
    public class UpdateMessageUsecase
    {
        IMessageRepo Repo { get; }

        IUserRepo UserRepo { get; }

        public enum Result
        {
            SUCCESS,
            SAME,
            EMPTY_MESSAGE,
            SENDER_NOT_FOUND,
            INVALID_PASSWORD,
            NOT_FOUND
        }

        public UpdateMessageUsecase(IMessageRepo repo, IUserRepo userRepo)
        {
            Repo = repo;
            UserRepo = userRepo;
        }

        public Result Execute(string message_id, string sender_id, string sender_password, string new_text, string new_timestamp)
        {
            if (UserRepo.GetUserById(sender_id) == null)
            {
                Console.WriteLine($"\nSender {sender_id} not found. (Message editing attempt)");

                return Result.SENDER_NOT_FOUND;
            }
            else if (UserRepo.GetUserById(sender_id).Password != sender_password)
            {
                Console.WriteLine($"\nPasswors not match. (Message editing attempt)");

                return Result.INVALID_PASSWORD;
            }
            else if (Repo.GetMessage(message_id) == null)
            {
                Console.WriteLine($"\nMessage id={message_id} doesn't exist. (Updating attempt)");

                return Result.NOT_FOUND;
            }
            else if (Repo.GetMessage(message_id).Text == new_text)
            {
                Console.WriteLine($"\nMessage id={message_id} has the same text. (Updating attempt)");

                return Result.SAME;
            }
            else if (new_text == "")
            {
                Console.WriteLine($"\nMessage id={message_id} cannot have the empty text. (Updating attempt)");

                return Result.EMPTY_MESSAGE;
            }
            else
            {
                Message _target = Repo.GetMessage(message_id);
                _target.Text = new_text;
                _target.Edited = true;
                _target.TimeStamp = new_timestamp;

                Repo.UpdateMessage(_target);

                Console.WriteLine($"\nMessage id={message_id} updated successfully.\nNew text: \"{new_text}\"\nEdit time: {new_timestamp}");

                return Result.SUCCESS;
            }
        }
    }
}
