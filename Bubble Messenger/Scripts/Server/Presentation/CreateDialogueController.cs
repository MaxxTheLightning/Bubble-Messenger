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

        public record DialogueDTO(string dialogue_name, string dialogue_type, string creator_id, string creator_password);

        public IResult Provide(DialogueDTO dto)
        {
            var result = Usecase.Execute(dto.dialogue_name, dto.dialogue_type, dto.creator_id, dto.creator_password);

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
