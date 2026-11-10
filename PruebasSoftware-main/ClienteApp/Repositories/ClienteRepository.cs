using ClienteApp.Core.Data;
using ClienteApp.Core.Models;

namespace ClienteApp.Core.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly string _connectionString;

    // Conexión predeterminada
    public ClienteRepository()
        : this("Data Source=clientes.db")
    {
    }

    // Conexión configurable
    public ClienteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Agregar(Cliente cliente)
    {
        using var conexion =
            Database.ObtenerConexion(_connectionString);

        using var comando = conexion.CreateCommand();

        comando.CommandText = """
            INSERT INTO Clientes (Nombre, Correo)
            VALUES ($nombre, $correo);

            SELECT last_insert_rowid();
            """;

        comando.Parameters.AddWithValue("$nombre", cliente.Nombre);
        comando.Parameters.AddWithValue("$correo", cliente.Correo);

        cliente.Id = Convert.ToInt32(comando.ExecuteScalar());
    }

    public Cliente? ObtenerPorId(int id)
    {
        using var conexion =
            Database.ObtenerConexion(_connectionString);

        using var comando = conexion.CreateCommand();

        comando.CommandText = """
            SELECT Id, Nombre, Correo
            FROM Clientes
            WHERE Id = $id;
            """;

        comando.Parameters.AddWithValue("$id", id);

        using var lector = comando.ExecuteReader();

        if (!lector.Read())
        {
            return null;
        }

        return new Cliente
        {
            Id = lector.GetInt32(0),
            Nombre = lector.GetString(1),
            Correo = lector.GetString(2)
        };
    }

    public bool ExisteCorreo(string correo)
    {
        using var conexion =
            Database.ObtenerConexion(_connectionString);

        using var comando = conexion.CreateCommand();

        comando.CommandText = """
            SELECT COUNT(*)
            FROM Clientes
            WHERE Correo = $correo COLLATE NOCASE;
            """;

        comando.Parameters.AddWithValue("$correo", correo);

        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }
}
