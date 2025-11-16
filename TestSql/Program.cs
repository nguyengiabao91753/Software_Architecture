using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var connectionString =
            "Server=localhost,1433;Database=master;Integrated Security=True;TrustServerCertificate=True;";

        try
        {
            Console.WriteLine("Connecting...");
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            Console.WriteLine("CONNECTED SUCCESS!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR:");
            Console.WriteLine(ex.ToString());
        }
    }
}
