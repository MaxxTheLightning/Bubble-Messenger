using Application;
using Domain;

namespace Presentation
{
    public class GetAccountController
    {
        GetAccountUsecase Usecase { get; }

        IUserRepo UserRepo { get; }

        public GetAccountController(GetAccountUsecase uc, IUserRepo userRepo)
        {
            Usecase = uc;
            UserRepo = userRepo;
        }

        public record Userdto(string id, string password);

        public IResult Provide(Userdto dto)
        {
            var result = Usecase.Execute(dto.id, dto.password);

            if (result == GetAccountUsecase.Result.SUCCESS)
            {
                User _user = UserRepo.GetUserById(dto.id);

                string response = "{ " + $"\"response\": \"Success.\", \"name\": \"{_user.Name}\", \"password\": \"{_user.Password}\", \"bio\": \"{_user.Bio}\", \"color\": \"{_user.Color}\", \"avatar\": \"{_user.AvatarUrl}\"" + " }";

                return Results.Ok(response);
            }
            else if (result == GetAccountUsecase.Result.INVALID_PASSWORD)
            {
                string response = "{ " + $"\"response\": \"Invalid password!\"" + " }";

                return Results.Conflict(response);
            }
            else
            {
                string response = "{ " + $"\"response\": \"User not found!\"" + " }";

                return Results.Conflict(response);
            }
        }
    }
}
