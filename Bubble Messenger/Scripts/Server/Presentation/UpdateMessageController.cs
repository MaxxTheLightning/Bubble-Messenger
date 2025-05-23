using Application;

namespace Presentation
{
    public class UpdateMessageController
    {
        UpdateMessageUsecase Usecase { get; set; }

        public UpdateMessageController(UpdateMessageUsecase uc)
        {
            Usecase = uc;
        }

        public record MessageDTo(string id, string new_text);

        public IResult Provide(MessageDTo dto)
        {
            var result = Usecase.Execute(dto.id, dto.new_text);

            if (result == UpdateMessageUsecase.Result.SUCCESS)
            {
                return Results.Ok("Message updated successfully!");
            }
            else if (result == UpdateMessageUsecase.Result.NOT_FOUND)
            {
                return Results.Conflict("Message not found.");
            }
            else if(result == UpdateMessageUsecase.Result.SAME)
            {
                return Results.Conflict("New text is the same.");
            }
            else
            {
                return Results.Conflict("The new message is empty.");
            }
        }
    }
}
