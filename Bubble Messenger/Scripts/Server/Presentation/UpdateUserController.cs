using Application;

namespace Presentation
{
    public class UpdateUserController
    {
        UpdateUserUsecase Usecase { get; set; }

        public UpdateUserController(UpdateUserUsecase uc)
        {
            Usecase = uc;
        }

        public record UserdtO(string old_name, string new_name, string password, string bio, string color);

        public IResult Provide(UserdtO dto)
        {
            var result = Usecase.Execute(dto.old_name, dto.new_name, dto.password, dto.bio, dto.color);

            if (result == UpdateUserUsecase.Result.SUCCESS)
            {
                return Results.Ok("User updated successfully!");
            }
            else if (result == UpdateUserUsecase.Result.NOT_FOUND)
            {
                return Results.Conflict("User not found.");
            }
            else
            {
                return Results.Conflict("Name already exists!");
            }
        }
    }
}
