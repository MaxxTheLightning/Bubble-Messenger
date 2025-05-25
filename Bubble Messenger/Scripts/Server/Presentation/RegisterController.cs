using Application;
using Microsoft.AspNetCore.Cors;

namespace Presentation
{
    public class RegisterController
    {
        RegisterUsecase Usecase { get; set; }

        public RegisterController(RegisterUsecase uc)
        {
            Usecase = uc;
        }

        public record UserDTO(string name, string password);

        [DisableCors]
        public IResult Provide(UserDTO dto)
        {
            var result = Usecase.Execute(dto.name, dto.password);

            if (result == RegisterUsecase.Result.SUCCESS)
            {
                return Results.Ok("User registrated successfully!");
            }
            else
            {
                return Results.Conflict("User already exists!");
            }
        }
    }
}
