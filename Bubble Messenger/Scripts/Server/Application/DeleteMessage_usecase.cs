using Domain;

namespace Application
{
    public class DeleteMessageUsecase
    {
        IMessageRepo Repo { get; }

        IUserRepo UserRepo { get; }

        public enum Result
        {
            SUCCESS,
            EMPTY_SENDER,
            INVALID_PASSWORD,
            MESSAGE_DOESNT_EXIST
        }

        public DeleteMessageUsecase(IMessageRepo repo, IUserRepo userRepo)
        {
            Repo = repo;
            UserRepo = userRepo;
        }

        public Result Execute(string message_id, string sender_id, string sender_password)
        {
            Message _message = Repo.GetMessage(message_id);  // Пробуем получить сообщение
            if (UserRepo.GetUserById(sender_id) == null)
            {
                Console.WriteLine($"\nError deleting message: empty sender.");

                return Result.EMPTY_SENDER;
            }
            else if (UserRepo.GetUserById(sender_id).Password != sender_password)
            {
                Console.WriteLine("\nPasswords don't match. (Deleting message attempt)");

                return Result.INVALID_PASSWORD;
            }
            else if (_message != null)
            {
                Repo.DeleteMessage(message_id);

                Console.WriteLine($"\nMessage id={message_id} deleted successfully.");

                return Result.SUCCESS;
            }
            else
            {
                Console.WriteLine($"\nMessage id={message_id} doesn't exist. (Deleting attempt)");

                return Result.MESSAGE_DOESNT_EXIST;
            }
        }
    }
}
