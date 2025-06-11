using Domain;
using System.Reflection;

namespace Application
{
    public class CreateMessageUsecase
    {
        IMessageRepo MessageRepo { get; }

        IUserRepo UserRepo { get; }

        IDialogueRepo DialogueRepo { get; }

        public enum Result
        {
            SUCCESS,
            ERROR
            // Возможно, стоит добавить обработку ошибок. Например, проверка существования пользователя и/или диалога.
        }

        public CreateMessageUsecase(IMessageRepo repo, IUserRepo userRepo, IDialogueRepo dialogueRepo)
        {
            MessageRepo = repo;
            UserRepo = userRepo;
            DialogueRepo = dialogueRepo;
        }

        public Result Execute(string sender_id, string sender_password, string receiver_id, string text, string time)
        {
            if (UserRepo.GetUserById(sender_id) != null && UserRepo.GetUserById(sender_id).Password == sender_password)
            {
                MessageRepo.CreateMessage(UserRepo.GetUserById(sender_id), DialogueRepo.GetDialogueById(receiver_id), text, time);

                return Result.SUCCESS;
            }
            else
            {
                return Result.ERROR;
            }
        }
    }
}