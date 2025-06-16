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

        CreateDialogueController CreateDialogueController { get; set; }

        DeleteDialogueController DeleteDialogueController { get; set; }

        UpdateDialogueController UpdateDialogueController { get; set; }

        GetAccountController GetAccountController { get; set; }

        ViewAccountController ViewAccountController { get; set; }

        GetDialogueController GetDialogueController { get; set; }

        ViewDialogueController ViewDialogueController { get; set; }

        GetChatsController GetChatsController { get; set; }

        GetChannelsController GetChannelsController { get; set; }

        GetUsersController GetUsersController { get; set; }

        GetMessagesController GetMessagesController { get; set; }

        public Routes(WebApplication application, RegisterController regController, LoginController log_control, DeleteUserController deleteUserController, CreateMessageController createMessageController, UpdateUserController updateUserController, UpdateMessageController updateMessageController, DeleteMessageController deleteMessageController, CreateDialogueController createDialogueController, DeleteDialogueController deleteDialogueController, UpdateDialogueController updateDialogueController, GetAccountController getAccountController, ViewAccountController viewAccountController, GetDialogueController getDialogueController, ViewDialogueController viewDialogueController, GetChatsController getChatsController, GetChannelsController getChannelsController, GetUsersController getUsersController, GetMessagesController getMessagesController)
        {
            Application = application;
            RegController = regController;
            LoginController = log_control;
            DeleteUserController = deleteUserController;
            CreateMessageController = createMessageController;
            UpdateUserController = updateUserController;
            UpdateMessageController = updateMessageController;
            DeleteMessageController = deleteMessageController;
            CreateDialogueController = createDialogueController;
            DeleteDialogueController = deleteDialogueController;
            UpdateDialogueController = updateDialogueController;
            GetAccountController = getAccountController;
            ViewAccountController = viewAccountController;
            GetDialogueController = getDialogueController;
            ViewDialogueController = viewDialogueController;
            GetChatsController = getChatsController;
            GetChannelsController = getChannelsController;
            GetUsersController = getUsersController;
            GetMessagesController = getMessagesController;
        }


        public void SetupRoutes()
        {
            Application.MapPost("/register", RegController.Provide);
            Application.MapPost("/login", LoginController.Provide);
            Application.MapPost("/delete_user", DeleteUserController.Provide);
            Application.MapPost("/update_user", UpdateUserController.Provide);
            Application.MapPost("/get_user", GetAccountController.Provide);
            Application.MapPost("/view_user", ViewAccountController.Provide);
            Application.MapPost("/get_users", GetUsersController.Provide);

            Application.MapPost("/send", CreateMessageController.Provide);
            Application.MapPost("/edit_message", UpdateMessageController.Provide);
            Application.MapPost("/delete_message", DeleteMessageController.Provide);
            Application.MapPost("/get_messages", GetMessagesController.Provide);

            Application.MapPost("/create_dialogue", CreateDialogueController.Provide);
            Application.MapPost("/delete_dialogue", DeleteDialogueController.Provide);
            Application.MapPost("/update_dialogue", UpdateDialogueController.Provide);
            Application.MapPost("/get_dialogue", GetDialogueController.Provide);
            Application.MapPost("/view_dialogue", ViewDialogueController.Provide);
            Application.MapPost("/get_chats", GetChatsController.Provide);
            Application.MapPost("/get_channels", GetChannelsController.Provide);
        }
    }
}
