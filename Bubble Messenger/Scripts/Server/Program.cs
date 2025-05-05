using Application;
using Infrastructure;
using Presentation;

var builder = WebApplication.CreateBuilder(args);

var id_provider = new RandomIdProvider();

var repo = new MockUserRepo(id_provider);

var register_uc = new RegisterUsecase(repo);

var login_uc = new LoginUsecase(repo);

var del_user_uc = new DeleteUserUsecase(repo);

var reg_control = new RegisterController(register_uc);

var log_control = new LoginController(login_uc);

var del_user_control = new DeleteUserController(del_user_uc);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var routes = new Routes(app, reg_control, log_control, del_user_control);

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

routes.SetupRoutes();

app.Run();