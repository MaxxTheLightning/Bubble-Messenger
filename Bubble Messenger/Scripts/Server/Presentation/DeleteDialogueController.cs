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

        public record DialogueDto(string id);

        public IResult Provide(DialogueDto dto)
        {
            var result = Usecase.Execute(dto.id);

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
