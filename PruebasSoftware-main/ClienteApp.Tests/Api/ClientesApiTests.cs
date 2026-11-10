
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ClienteApp.Tests.Api;

public class ClientesApiTests
{
    [Fact]
    public async Task Registrar_ClienteValido_DebeRetornar201()
    {
        // Arrange
        using var factory = new ClienteApiFactory();
        using var client = factory.CreateClient();

        var solicitud = new
        {
            Nombre = "Ana López",
            Correo = $"ana_{Guid.NewGuid():N}@example.com"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/Clientes",
            solicitud);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var cliente = await response.Content
            .ReadFromJsonAsync<ClienteRespuesta>();

        Assert.NotNull(cliente);
        Assert.Equal(solicitud.Nombre, cliente.Nombre);
        Assert.Equal(solicitud.Correo, cliente.Correo);
    }

    [Fact]
    public async Task Registrar_NombreVacio_DebeRetornar400()
    {
        // Arrange
        using var factory = new ClienteApiFactory();
        using var client = factory.CreateClient();

        var solicitud = new
        {
            Nombre = "",
            Correo = $"correo_{Guid.NewGuid():N}@example.com"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/Clientes",
            solicitud);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private sealed record ClienteRespuesta(
        int Id,
        string Nombre,
        string Correo);
}

public sealed class ClienteApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(
        Path.GetTempPath(),
        $"clientes_api_test_{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:ClientesDb"] =
                        $"Data Source={_dbPath};Pooling=False"
                });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }
}
