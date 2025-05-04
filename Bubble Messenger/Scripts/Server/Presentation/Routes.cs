namespace Presentation
{
    public class Routes
    {
        WebApplication Application { get; set; }

        RegisterController RegController { get; set; }

        public Routes(WebApplication application, RegisterController regController)
        {
            Application = application;
            RegController = regController;
        }

        public void SetupRoutes()
        {
            Application.MapPost("/register", RegController.Provide);
        }
    }
}
