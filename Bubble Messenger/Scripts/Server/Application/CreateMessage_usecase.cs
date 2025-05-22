using Domain;

namespace Application
{
    public class CreateMessageUsecase
    {
        IMessageRepo MessageRepo { get; }

        IUserRepo UserRepo { get; }

        IDialogueRepo DialogueRepo { get; }

        public enum Result
        {
            SUCCESS

            // Возможно, стоит добавить обработку ошибок. Например, проверка существования пользователя и/или диалога.
        }

        public CreateMessageUsecase(IMessageRepo repo, IUserRepo userRepo, IDialogueRepo dialogueRepo)
        {
            MessageRepo = repo;
            UserRepo = userRepo;
            DialogueRepo = dialogueRepo;
        }

        public Result Execute(string sender, string receiver, string text, string time, string json)
        {
            MessageRepo.CreateMessage(UserRepo.GetUserByName(sender), DialogueRepo.GetDialogueByName(receiver), text, time, json);

            return Result.SUCCESS;
        }
    }
}
