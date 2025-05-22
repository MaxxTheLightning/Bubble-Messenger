namespace Presentation
{
    public class Routes
    {
        WebApplication Application { get; set; }

        RegisterController RegController { get; set; }

        LoginController LoginController { get; set; }

        DeleteUserController DeleteUserController { get; set; }

        CreateMessageController CreateMessageController { get; set; }

        public Routes(WebApplication application, RegisterController regController, LoginController log_control, DeleteUserController deleteUserController, CreateMessageController createMessageController)
        {
            Application = application;
            RegController = regController;
            LoginController = log_control;
            DeleteUserController = deleteUserController;
            CreateMessageController = createMessageController;
        }

        public void SetupRoutes()
        {
            Application.MapPost("/register", RegController.Provide);
            Application.MapPost("/login", LoginController.Provide);
            Application.MapPost("/delete_user", DeleteUserController.Provide);
            Application.MapPost("/send_message", CreateMessageController.Provide);
        }
    }
}
