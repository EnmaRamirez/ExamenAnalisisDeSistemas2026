using Microsoft.Data.Sqlite;

namespace ClienteApp.Core.Data;

public static class Database
{
    public static SqliteConnection ObtenerConexion(
        string connectionString)
    {
        var conexion = new SqliteConnection(connectionString);
        conexion.Open();
        return conexion;
    }

    public static void Inicializar(
        string connectionString)
    {
        using var conexion = ObtenerConexion(connectionString);
        using var comando = conexion.CreateCommand();

        comando.CommandText = """
            CREATE TABLE IF NOT EXISTS Clientes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                Correo TEXT NOT NULL UNIQUE
            );
            """;

        comando.ExecuteNonQuery();
    }
}