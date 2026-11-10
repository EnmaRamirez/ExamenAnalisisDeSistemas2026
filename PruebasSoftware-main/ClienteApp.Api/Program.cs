using ClienteApp.Core.Data;
using ClienteApp.Core.Models;
using ClienteApp.Core.Repositories;
using ClienteApp.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios de ASP.NET Core
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Obtener la cadena de conexión desde appsettings.json
var connectionString =
    builder.Configuration.GetConnectionString("ClientesDb")
    ?? throw new InvalidOperationException(
        "No se configuró la cadena de conexión ClientesDb.");

// Registrar dependencias de la aplicación
builder.Services.AddScoped<IClienteRepository>(
    _ => new ClienteRepository(connectionString));

builder.Services.AddScoped<ClienteService>();

var app = builder.Build();

// Inicializar la base de datos con la conexión configurada
Database.Inicializar(connectionString);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
