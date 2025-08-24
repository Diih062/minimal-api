using MinimalApi.Dominio.DTOs;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Dominio.ModelViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
    /*
    var connectionString = builder.Configuration.GetConnectionString("mysql") ?? 
        throw new InvalidOperationException("Connection string 'mysql' not found.");
    
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
    */
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    //if (loginDTO.Email == "admin@teste.com" && loginDTO.Senha == "123456")
    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login realizado com sucesso!");
    else
        return Results.Unauthorized();
});
#endregion

#region Veiculos

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
});

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Created($"/veiculos", veiculos);
});

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
