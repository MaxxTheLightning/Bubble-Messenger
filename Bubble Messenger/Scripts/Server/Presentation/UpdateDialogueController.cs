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

        public record DialogueDtO(string id, string new_name);

        public IResult Rename(DialogueDtO dto)
        {
            var result = Usecase.Rename(dto.id, dto.new_name);

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
                return Results.Conflict("The new dialogue name is empty.");
            }
        }
    }
}
