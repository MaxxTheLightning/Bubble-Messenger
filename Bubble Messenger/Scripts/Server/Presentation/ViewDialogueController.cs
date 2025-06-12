using Application;
using Domain;

namespace Presentation
{
    public class ViewDialogueController
    {
        ViewDialogueUsecase Usecase { get; }

        IDialogueRepo DialogueRepo { get; }

        public ViewDialogueController(ViewDialogueUsecase uc, IDialogueRepo userRepo)
        {
            Usecase = uc;
            DialogueRepo = userRepo;
        }

        public record Dialogue_DTO(string id);

        public IResult Provide(Dialogue_DTO dto)
        {
            var result = Usecase.Execute(dto.id);

            if (result == ViewDialogueUsecase.Result.SUCCESS)
            {
                Dialogue _dialogue = DialogueRepo.GetDialogueById(dto.id);

                string participants = "[";
                string administrators = "[";

                foreach (User u in _dialogue.Participants)
                {
                    participants += $"[{u.Name}, {u.Id}], ";
                }

                string new_participants = participants[..-2];

                new_participants += "]";

                foreach (User u in _dialogue.Administrators)
                {
                    administrators += $"[{u.Name}, {u.Id}], ";
                }

                string new_admins = administrators[..-2];

                new_admins += "]";

                string response = "{ " + $"\"response\": \"Success.\", \"name\": \"{_dialogue.Name}\", \"type\": \"{_dialogue.Type}\", \"bio\": \"{_dialogue.Bio}\", \"avatar\": \"{_dialogue.AvatarUrl}\", \"participants\": \"{new_participants}\", \"admins\": \"{new_admins}\"" + " }";

                return Results.Ok(response);
            }
            else
            {
                string response = "{ " + $"\"response\": \"Dialogue not found!\"" + " }";

                return Results.Conflict(response);
            }
        }
    }
}
