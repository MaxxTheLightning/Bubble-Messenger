using Application;

namespace Presentation
{
    public class UpdateDialogueController
    {
        UpdateDialogueUsecase Usecase { get; set; }

        public UpdateDialogueController(UpdateDialogueUsecase uc)
        {
            Usecase = uc;
        }

        public record DialogueDtO(string id, string author_id, string author_password, string new_name, string about, string avatarUrl, string participants_IDs, string admins_IDs, string banned_IDs, string muted_IDs);

        public IResult Provide(DialogueDtO dto)
        {
            var result = Usecase.Execute(dto.id, dto.author_id, dto.author_password, dto.new_name, dto.about, dto.avatarUrl, dto.participants_IDs, dto.admins_IDs, dto.banned_IDs, dto.muted_IDs);

            if (result == UpdateDialogueUsecase.Result.SUCCESS)
            {
                return Results.Ok("Dialogue updated successfully!");
            }
            else if (result == UpdateDialogueUsecase.Result.DIALOGUE_NOT_FOUND)
            {
                return Results.Conflict("Dialogue not found.");
            }
            else if (result == UpdateDialogueUsecase.Result.SAME_NAME)
            {
                return Results.Conflict("New dialogue name is the same.");
            }
            else
            {
                return Results.Conflict("Error.");
            }
        }
    }
}
