using Application;

namespace Presentation
{
    public class CreateMessageController
    {
        CreateMessageUsecase Usecase { get; set; }

        public CreateMessageController(CreateMessageUsecase uc)
        {
            Usecase = uc;
        }

        public record MessageDTO(string sender_id, string sender_password, string receiver_id, string text, string time);

        public IResult Provide(MessageDTO dto)
        {
            var result = Usecase.Execute(dto.sender_id, dto.sender_password, dto.receiver_id, dto.text, dto.time);

            if (result == CreateMessageUsecase.Result.SUCCESS)
            {
                return Results.Ok("Message sent successfully!");
            }
            else
            {
                return Results.Conflict("Message sending failed!");
            }
        }
    }
}
