using Application;
using Domain;

namespace Presentation
{
    public class GetDialogueController
    {
        GetDialogueUsecase Usecase { get; }

        IDialogueRepo DialogueRepo { get; }

        public GetDialogueController(GetDialogueUsecase uc, IDialogueRepo dialogueRepo)
        {
            Usecase = uc;
            DialogueRepo = dialogueRepo;
        }

        public record Dialoguedto(string dialogue_id, string creator_id, string password);

        public IResult Provide(Dialoguedto dto)
        {
            var result = Usecase.Execute(dto.dialogue_id, dto.creator_id, dto.password);

            if (result == GetDialogueUsecase.Result.SUCCESS)
            {
                Dialogue _dialogue = DialogueRepo.GetDialogueById(dto.dialogue_id);

                string dialogue_name = _dialogue.Name;
                string dialogue_bio = _dialogue.Bio;
                string dialogue_avatar = _dialogue.AvatarUrl;
                string participants = "[";
                string administrators = "[";
                string banned = "[";
                string muted = "[";

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

                foreach (User u in _dialogue.Banned)
                {
                    banned += $"[{u.Name}, {u.Id}], ";
                }

                string new_banned = banned[..-2];

                new_banned += "]";

                foreach (User u in _dialogue.Muted)
                {
                    muted += $"[{u.Name}, {u.Id}], ";
                }

                string new_muted = muted[..-2];

                new_muted += "]";

                string response = "{ " + $"\"response\": \"Success.\", \"name\": \"{_dialogue.Name}\", \"bio\": \"{_dialogue.Bio}\", \"avatar\": \"{_dialogue.AvatarUrl}\", \"participants\": \"{new_participants}\", \"admins\": \"{new_admins}\", \"banned\": \"{new_banned}\"" + " }";

                return Results.Ok(response);
            }
            else if (result == GetDialogueUsecase.Result.INVALID_PASSWORD)
            {
                string response = "{ " + $"\"response\": \"Invalid password!\"" + " }";

                return Results.Conflict(response);
            }
            else if (result == GetDialogueUsecase.Result.CREATOR_NOT_FOUND)
            {
                string response = "{ " + $"\"response\": \"User not found!\"" + " }";

                return Results.Conflict(response);
            }
            else
            {
                string response = "{ " + $"\"response\": \"Dialogue not found!\"" + " }";

                return Results.Conflict(response);
            }
        }
    }
}
