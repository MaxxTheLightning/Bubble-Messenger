namespace Presentation
{
    public class Routes
    {
        WebApplication Application { get; set; }

        RegisterController RegController { get; set; }

        LoginController LoginController { get; set; }

        UpdateUserController UpdateUserController { get; set; }

        DeleteUserController DeleteUserController { get; set; }

        CreateMessageController CreateMessageController { get; set; }

        UpdateMessageController UpdateMessageController { get; set; }

        DeleteMessageController DeleteMessageController { get; set; }

        public Routes(WebApplication application, RegisterController regController, LoginController log_control, DeleteUserController deleteUserController, CreateMessageController createMessageController, UpdateUserController updateUserController, UpdateMessageController updateMessageController, DeleteMessageController deleteMessageController)
        {
            Application = application;
            RegController = regController;
            LoginController = log_control;
            DeleteUserController = deleteUserController;
            CreateMessageController = createMessageController;
            UpdateUserController = updateUserController;
            UpdateMessageController = updateMessageController;
            DeleteMessageController = deleteMessageController;
        }

        public void SetupRoutes()
        {
            Application.MapPost("/register", RegController.Provide);
            Application.MapPost("/login", LoginController.Provide);
            Application.MapPost("/delete_user", DeleteUserController.Provide);
            Application.MapPost("/send_message", CreateMessageController.Provide);
            Application.MapPost("/update_user", UpdateUserController.Provide);
            Application.MapPost("/update_message", UpdateMessageController.Provide);
            Application.MapPost("/delete_message", DeleteMessageController.Provide);
        }
    }
}
