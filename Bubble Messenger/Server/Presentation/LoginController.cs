using Application;
using Domain;

namespace Presentation
{
    public class LoginController
    {
        LoginUsecase Usecase { get; set; }

        IUserRepo UserRepo { get; set; }

        public LoginController(LoginUsecase uc, IUserRepo repo)
        {
            Usecase = uc;
            UserRepo = repo;
        }

        public record UserDTo(string name, string password);

        public IResult Provide(UserDTo dto)
        {
            var result = Usecase.Execute(dto.name, dto.password);

            if (result == LoginUsecase.Result.SUCCESS)
            {
                string response = "{ " + $"\"response\": \"User loginned successfully!\", \"id\": \"{UserRepo.GetUserByName(dto.name).Id}\"" + " }";

                return Results.Ok(response);
            }
            else if (result == LoginUsecase.Result.INVALID_PASSWORD)
            {
                string response = "{ " + $"\"response\": \"Invalid password!\"" + " }";

                return Results.Conflict(response);
            }
            else
            {
                string response = "{ " + $"\"response\": \"User doesn't exist!\"" + " }";

                return Results.Conflict(response);
            }
        }
    }
}
