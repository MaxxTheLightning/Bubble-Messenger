using Domain;
using System.Text.RegularExpressions;

namespace Presentation
{
    public class GetMessagesController
    {
        IDialogueRepo DialogueRepo { get; }

        public GetMessagesController(IDialogueRepo dialogueRepo)
        {
            DialogueRepo = dialogueRepo;
        }

        public record GetMessagesDTO (string dialogue_id);

        public IResult Provide (GetMessagesDTO dto)
        {
            Dialogue _dialogue = DialogueRepo.GetDialogueById(dto.dialogue_id);

            if (_dialogue.Messages.Count > 0)
            {
                string response = "{ ";

                int count = 1;

                foreach (Message m in _dialogue.Messages)
                {
                    response += $"\"{count}\": \"" + Regex.Replace(m.Json, "\"", "\\\"") + "\", ";
                    count++;
                }

                string new_response = response[..(response.Length - 2)];

                new_response += " }";

                return Results.Ok(new_response);
            }
            else
            {
                return Results.Conflict("No messages yet");
            }
        }
    }
}
