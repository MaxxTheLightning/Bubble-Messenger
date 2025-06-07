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

        public record UserdtO(string id, string password, string new_name, string new_password, string new_bio, string new_color, string new_avatarUrl);

        public IResult Provide(UserdtO dto)
        {
            var result = Usecase.Execute(dto.id, dto.password, dto.new_name, dto.new_password, dto.new_bio, dto.new_color, dto.new_avatarUrl);

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
