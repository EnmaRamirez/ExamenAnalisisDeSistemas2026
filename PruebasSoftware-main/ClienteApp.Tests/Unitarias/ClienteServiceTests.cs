using ClienteApp.Core.Models;
using ClienteApp.Core.Repositories;
using ClienteApp.Core.Services;
using Moq;

namespace ClienteApp.Tests.Unitarias;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _service = new ClienteService(_repositoryMock.Object);
    }

    [Fact]
    public void EsCorreoValido_CorreoCorrecto_DebeRetornarTrue()
    {
        // Act
        var resultado = _service.EsCorreoValido("cliente@gmail.com");

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void EsCorreoValido_CorreoSinArroba_DebeRetornarFalse()
    {
        // Act
        var resultado = _service.EsCorreoValido("cliente");

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void EsCorreoValido_CorreoVacio_DebeRetornarFalse()
    {
        // Act
        var resultado = _service.EsCorreoValido("");

        // Assert
        Assert.False(resultado);
    }

    [Theory]
    [InlineData("cliente@gmail.com", true)]
    [InlineData("cliente", false)]
    [InlineData("", false)]
    public void EsCorreoValido_DebeRetornarResultadoEsperado(
        string correo,
        bool esperado)
    {
        // Act
        var resultado = _service.EsCorreoValido(correo);

        // Assert
        Assert.Equal(esperado, resultado);
    }


    [Fact]
    public void Registrar_DatosValidos_DebeRegistrarCliente()
    {
        // Arrange: preparar el escenario
        _repositoryMock.Setup(r => r.ExisteCorreo("ana@example.com")).Returns(false);

        var nombre = " Ana López ";
        var correo = "ana@example.com";

        // Act: ejecutar el método que probamos
        var cliente = _service.Registrar(nombre, correo);

        // Assert: comprobar los resultados
        Assert.NotNull(cliente);
        Assert.Equal("Ana López", cliente.Nombre);
        Assert.Equal(correo, cliente.Correo);

        _repositoryMock.Verify(
            r => r.Agregar(It.Is<Cliente>(c =>
                c.Nombre == "Ana López" &&
                c.Correo == correo)),
            Times.Once);
    }


    [Fact]
    public void Registrar_NombreVacio_DebeLanzarExcepcion()
    {
        var excepcion = Assert.Throws<ArgumentException>(
            () => _service.Registrar("", "ana@example.com"));

        Assert.Equal("El nombre es obligatorio.", excepcion.Message);

        _repositoryMock.Verify(
            r => r.Agregar(It.IsAny<Cliente>()),
            Times.Never);
    }

    [Fact]
    public void Registrar_CorreoInvalido_DebeLanzarExcepcion()
    {
        var excepcion = Assert.Throws<ArgumentException>(
            () => _service.Registrar("Ana López", "correo-invalido"));

        Assert.Equal("El correo no es válido.", excepcion.Message);

        _repositoryMock.Verify(
            r => r.Agregar(It.IsAny<Cliente>()),
            Times.Never);
    }

    [Fact]
    public void Registrar_CorreoDuplicado_DebeLanzarExcepcion()
    {
        _repositoryMock
            .Setup(r => r.ExisteCorreo("ana@example.com"))
            .Returns(true);

        var excepcion = Assert.Throws<InvalidOperationException>(
            () => _service.Registrar("Ana López", "ana@example.com"));

        Assert.Equal("El correo ya está registrado.", excepcion.Message);

        _repositoryMock.Verify(
            r => r.Agregar(It.IsAny<Cliente>()),
            Times.Never);
    }
        [Fact]
    public void ObtenerPorId_DebeDevolverClienteYConsultarRepositorioUnaVez()
    {
        // Arrange
        var clienteEsperado = _service.Registrar("Ana López", "ana@example.com");
        
        _repositoryMock.Setup(r => r.ObtenerPorId(clienteEsperado.Id))
                       .Returns(clienteEsperado);

        // Act
        var resultado = _service.ObtenerPorId(clienteEsperado.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(clienteEsperado.Id, resultado.Id);
        Assert.Equal(clienteEsperado.Nombre, resultado.Nombre);
        
        // Comprueba que consulta al repositorio exactamente una vez
        _repositoryMock.Verify(r => r.ObtenerPorId(clienteEsperado.Id), Times.Once);
    }



}
