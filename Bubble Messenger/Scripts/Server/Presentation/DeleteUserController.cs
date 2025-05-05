using Application;

namespace Presentation
{
    public class DeleteUserController
    {
        DeleteUserUsecase Usecase { get; set; }

        public DeleteUserController(DeleteUserUsecase uc)
        {
            Usecase = uc;
        }

        public record UserDto(string name, string password);

        public IResult Provide(UserDto dto)
        {
            var result = Usecase.Execute(dto.name, dto.password);

            if (result == DeleteUserUsecase.Result.SUCCESS)
            {
                return Results.Ok("User deleted successfully!");
            }
            else if (result == DeleteUserUsecase.Result.INVALID_PASSWORD)
            {
                return Results.Conflict("Invalid password!");
            }
            else
            {
                return Results.Conflict("User doesn't exist!");
            }
        }
    }
}
