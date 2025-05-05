using Application;

namespace Presentation
{
    public class LoginController
    {
        LoginUsecase Usecase { get; set; }

        public LoginController(LoginUsecase uc)
        {
            Usecase = uc;
        }

        public record UserDTo(string name, string password);

        public IResult Provide(UserDTo dto)
        {
            var result = Usecase.Execute(dto.name, dto.password);

            if (result == LoginUsecase.Result.SUCCESS)
            {
                return Results.Ok("User loginned successfully!");
            }
            else if (result == LoginUsecase.Result.INVALID_PASSWORD)
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
