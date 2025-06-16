using Application;
using Domain;

namespace Presentation
{
    public class ViewAccountController
    {
        ViewAccountUsecase Usecase { get; }

        IUserRepo UserRepo { get; }

        public ViewAccountController(ViewAccountUsecase uc, IUserRepo userRepo)
        {
            Usecase = uc;
            UserRepo = userRepo;
        }

        public record UserdTo(string id, string password);

        public IResult Provide(UserdTo dto)
        {
            var result = Usecase.Execute(dto.id);

            if (result == ViewAccountUsecase.Result.SUCCESS)
            {
                User _user = UserRepo.GetUserById(dto.id);

                string response = "{ " + $"\"response\": \"Success.\", \"name\": \"{_user.Name}\", \"bio\": \"{_user.Bio}\", \"color\": \"{_user.Color}\", \"avatar\": \"{_user.AvatarUrl}\"" + " }";

                return Results.Ok(response);
            }
            else
            {
                string response = "{ " + $"\"response\": \"User not found!\"" + " }";

                return Results.Conflict(response);
            }
        }
    }
}
