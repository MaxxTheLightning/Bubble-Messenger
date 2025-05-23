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

        public record MessageDto(string id);

        public IResult Provide(MessageDto dto)
        {
            var result = Usecase.Execute(dto.id);

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
