var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (APIMINIMADIO.DTOs.LoginDTO loginDTO) =>
{
    if(loginDTO.Email == "email@hotmail.com" && loginDTO.Senha == "123456")
        return Results.Ok("Login efeutado com sucesso");
    else
        return Results.Unauthorized();
});

app.MapPost("/todos", (APIMINIMADIO.DTOs.LoginDTO mostrarDTO) =>
{
    var resultado = new 
    {
        Email = mostrarDTO.Email,
        Senha = mostrarDTO.Senha
    };

    return resultado;

});

app.Run();


