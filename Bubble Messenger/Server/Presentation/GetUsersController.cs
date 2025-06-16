using Domain;

namespace Presentation
{
    public class GetUsersController
    {
        IUserRepo UserRepo { get; }

        public GetUsersController(IUserRepo userRepo)
        {
            UserRepo = userRepo;
        }

        public record GetPeopleDTO (string user_id);

        public IResult Provide(GetPeopleDTO dto)
        {
            List<User> _allUsers = UserRepo.GetAllUsers();

            _allUsers.Remove(UserRepo.GetUserById(dto.user_id));

            string response = "[";

            foreach (User _user in _allUsers)
            {
                response += $"[{_user.Name}, {_user.Id}, {_user.AvatarUrl}], ";
            }

            if (response.Length > 1)
            {
                string new_response = response[..(response.Length - 2)];

                new_response += "]";

                return Results.Ok(new_response);
            }
            else
            {
                return Results.Conflict("No users to display");
            }
        }
    }
}
