using ClienteApp.Core.Models;

namespace ClienteApp.Core.Services;

public class ClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public bool EsCorreoValido(string correo)
    {
        return !string.IsNullOrWhiteSpace(correo)
               && correo.Contains('@');
    }

    public Cliente Registrar(string nombre, string correo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException(
                "El nombre es obligatorio.");
        }

        if (!EsCorreoValido(correo))
        {
            throw new ArgumentException(
                "El correo no es válido.");
        }

        if (_repository.ExisteCorreo(correo))
        {
            throw new InvalidOperationException(
                "El correo ya está registrado.");
        }

        var cliente = new Cliente
        {
            Nombre = nombre.Trim(),
            Correo = correo.Trim()
        };

        _repository.Agregar(cliente);

        return cliente;
    }

    public Cliente? ObtenerPorId(int id)
    {
        return _repository.ObtenerPorId(id);
    }
}
