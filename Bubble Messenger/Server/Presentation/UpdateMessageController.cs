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

        public record MessageDTo(string message_id, string sender_id, string sender_password, string new_text, string new_timestamp);

        public IResult Provide(MessageDTo dto)
        {
            var result = Usecase.Execute(dto.message_id, dto.sender_id, dto.sender_password, dto.new_text, dto.new_timestamp);

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
