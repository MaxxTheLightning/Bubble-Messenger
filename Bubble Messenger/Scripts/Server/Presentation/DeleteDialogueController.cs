using Application;

namespace Presentation
{
    public class DeleteDialogueController
    {
        DeleteDialogueUsecase Usecase { get; set; }

        public DeleteDialogueController(DeleteDialogueUsecase uc)
        {
            Usecase = uc;
        }

        public record DialogueDto(string dialogue_id, string creator_id, string creator_password);

        public IResult Provide(DialogueDto dto)
        {
            var result = Usecase.Execute(dto.dialogue_id, dto.creator_id, dto.creator_password);

            if (result == DeleteDialogueUsecase.Result.SUCCESS)
            {
                return Results.Ok("Dialogue deleted successfully!");
            }
            else
            {
                return Results.Conflict("Dialogue doesn't exist!");
            }
        }
    }
}
