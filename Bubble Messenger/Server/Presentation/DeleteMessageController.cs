using Application;

namespace Presentation
{
    public class DeleteMessageController
    {
        DeleteMessageUsecase Usecase { get; set; }

        public DeleteMessageController(DeleteMessageUsecase uc)
        {
            Usecase = uc;
        }

        public record MessageDto(string message_id, string sender_id, string sender_password);

        public IResult Provide(MessageDto dto)
        {
            var result = Usecase.Execute(dto.message_id, dto.sender_id, dto.sender_password);

            if (result == DeleteMessageUsecase.Result.SUCCESS)
            {
                return Results.Ok("Message deleted successfully!");
            }
            else
            {
                return Results.Conflict("Message doesn't exist!");
            }
        }
    }
}
