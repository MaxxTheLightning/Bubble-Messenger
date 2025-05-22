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

        public record MessageDTO(string sender, string receiver, string text, string time, string json);

        public IResult Provide(MessageDTO dto)
        {
            var result = Usecase.Execute(dto.sender, dto.receiver, dto.text, dto.time, dto.json);

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
