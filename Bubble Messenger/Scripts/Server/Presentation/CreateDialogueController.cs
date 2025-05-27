using Application;

namespace Presentation
{
    public class CreateDialogueController
    {
        CreateDialogueUsecase Usecase { get; set; }

        public CreateDialogueController(CreateDialogueUsecase uc)
        {
            Usecase = uc;
        }

        public record DialogueDTO(string name, string type, string creator_name);

        public IResult Provide(DialogueDTO dto)
        {
            var result = Usecase.Execute(dto.name, dto.type, dto.creator_name);

            if (result == CreateDialogueUsecase.Result.SUCCESS)
            {
                return Results.Ok("Dialogue created successfully!");
            }
            else
            {
                return Results.Conflict("Dialogue name is not unique!");
            }
        }
    }
}
