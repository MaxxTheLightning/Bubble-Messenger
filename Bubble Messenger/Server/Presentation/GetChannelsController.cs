using Domain;

namespace Presentation
{
    public class GetChannelsController
    {
        IDialogueRepo DialogueRepo { get; }

        IUserRepo UserRepo { get; }

        public GetChannelsController(IDialogueRepo dialogueRepo, IUserRepo userRepo)
        {
            DialogueRepo = dialogueRepo;
            UserRepo = userRepo;
        }

        public record GetUserDTo (string user_id);

        public IResult Provide(GetUserDTo dto)
        {
            List<Dialogue> _allDialogues = DialogueRepo.GetAllDialogues();

            string response = "[";

            foreach (Dialogue _dialogue in _allDialogues)
            {
                if (_dialogue.Type == "channel" && _dialogue.Participants.Contains(UserRepo.GetUserById(dto.user_id)))
                {
                    response += $"[{_dialogue.Name}, {_dialogue.Id}, {_dialogue.AvatarUrl}], ";
                }
            }

            if (response.Length > 1)
            {
                string new_response = response[..(response.Length - 2)];

                new_response += "]";

                return Results.Ok(new_response);
            }
            else
            {
                return Results.Conflict("No channels to display.");
            }
        }
    }
}
