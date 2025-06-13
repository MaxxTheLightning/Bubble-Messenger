using Domain;

namespace Presentation
{
    public class GetChatsController
    {
        IDialogueRepo DialogueRepo { get; }

        public GetChatsController(IDialogueRepo dialogueRepo)
        {
            DialogueRepo = dialogueRepo;
        }

        public IResult Provide()
        {
            List<Dialogue> _allDialogues = DialogueRepo.GetAllDialogues();

            string response = "[";

            foreach (Dialogue _dialogue in  _allDialogues)
            {
                if (_dialogue.Type == "chat")
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
                string new_response = "Empty";

                return Results.Ok(new_response);
            }
        }
    }
}
