using Application;
using Infrastructure;
using Presentation;

var id_provider = new RandomIdProvider();

var user_repo = new MockUserRepo(id_provider);

WebSocketSpace websock = new WebSocketSpace(user_repo);

websock.Start();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://bubble-messenger.github.io")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .WithExposedHeaders("Access-Control-Allow-Private-Network");
    });
});



var broadcast_msg = new BroadcastMessage();


var msg_repo = new MockMessageRepo(id_provider, broadcast_msg);

var dialogue_repo = new MockDialogueRepo(id_provider);

var register_uc = new RegisterUsecase(user_repo);

var login_uc = new LoginUsecase(user_repo);

var del_user_uc = new DeleteUserUsecase(user_repo);

var update_user_uc = new UpdateUserUsecase(user_repo);

var create_message_uc = new CreateMessageUsecase(msg_repo, user_repo, dialogue_repo);

var update_message_uc = new UpdateMessageUsecase(msg_repo, user_repo);

var delete_message_uc = new DeleteMessageUsecase(msg_repo, user_repo);

var create_dialogue_uc = new CreateDialogueUsecase(dialogue_repo, user_repo);

var delete_dialogue_uc = new DeleteDialogueUsecase(dialogue_repo, user_repo);

var update_dialogue_uc = new UpdateDialogueUsecase(dialogue_repo, user_repo);

var get_account_uc = new GetAccountUsecase(user_repo);

var view_account_uc = new ViewAccountUsecase(user_repo);

var get_dialogue_uc = new GetDialogueUsecase(user_repo, dialogue_repo);

var view_dialogue_uc = new ViewDialogueUsecase(dialogue_repo);

var reg_control = new RegisterController(register_uc);

var log_control = new LoginController(login_uc, user_repo);

var del_user_control = new DeleteUserController(del_user_uc);

var update_user_control = new UpdateUserController(update_user_uc);

var create_message_control = new CreateMessageController(create_message_uc);

var update_message_control = new UpdateMessageController(update_message_uc);

var delete_message_control = new DeleteMessageController(delete_message_uc);

var create_dialogue_control = new CreateDialogueController(create_dialogue_uc);

var delete_dialogue_control = new DeleteDialogueController(delete_dialogue_uc);

var update_dialogue_control = new UpdateDialogueController(update_dialogue_uc);

var get_account_control = new GetAccountController(get_account_uc, user_repo);

var view_account_control = new ViewAccountController(view_account_uc, user_repo);

var get_dialogue_control = new GetDialogueController(get_dialogue_uc, dialogue_repo);

var view_dialogue_control = new ViewDialogueController(view_dialogue_uc, dialogue_repo);

var get_chats_control = new GetChatsController(dialogue_repo);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var routes = new Routes(app, reg_control, log_control, del_user_control, create_message_control, update_user_control, update_message_control, delete_message_control, create_dialogue_control, delete_dialogue_control, update_dialogue_control, get_account_control, view_account_control, get_dialogue_control, view_dialogue_control, get_chats_control);

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Private-Network", "true");
    }

    await next();
});

app.UseCors(); // Œ¡ﬂ«¿“≈À‹ÕŒ Á‰ÂÒ¸

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

routes.SetupRoutes();

app.Run();