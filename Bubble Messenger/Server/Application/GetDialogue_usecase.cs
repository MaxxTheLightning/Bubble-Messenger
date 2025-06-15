using Domain;

namespace Application
{
    public class GetDialogueUsecase
    {
        IUserRepo UserRepo { get; }

        IDialogueRepo DialogueRepo { get; }

        public enum Result
        {
            DIALOGUE_NOT_FOUND,
            CREATOR_NOT_FOUND,
            INVALID_PASSWORD,
            SUCCESS
        }

        public GetDialogueUsecase(IUserRepo userRepo, IDialogueRepo dialogueRepo)
        {
            UserRepo = userRepo;
            DialogueRepo = dialogueRepo;
        }

        public Result Execute(string dialogue_id, string creator_id, string password)
        {
            User _creator = UserRepo.GetUserById(creator_id);

            Dialogue _dialogue = DialogueRepo.GetDialogueById(dialogue_id);

            if (_creator != null)
            {
                if (_dialogue != null)
                {
                    if (_creator.Password == password)
                    {
                        return Result.SUCCESS;
                    }
                    else
                    {
                        Console.WriteLine($"\nInvalid password of {_creator.Name}. (Get dialogue attempt)\nEntered password: {password}\nCorrect password: {_creator.Password}");

                        return Result.INVALID_PASSWORD;
                    }
                }
                else
                {
                    Console.WriteLine($"\nDialogue {dialogue_id} doesn't exist. (Get dialogue attempt)");

                    return Result.DIALOGUE_NOT_FOUND;
                }
            }
            else
            {
                Console.WriteLine($"\nUser {creator_id} doesn't exist. (Get dialogue attempt)");

                return Result.CREATOR_NOT_FOUND;
            }
        }
    }
}
