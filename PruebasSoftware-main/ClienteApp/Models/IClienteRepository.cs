namespace ClienteApp.Core.Models;

public interface IClienteRepository
{
    void Agregar(Cliente cliente);

    Cliente? ObtenerPorId(int id);

    bool ExisteCorreo(string correo);
}
