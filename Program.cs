using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace npgsqlTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "";
            using (var pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                using(var pgTransaction = pgConnection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand("SELECT state,COUNT(1),pg_is_in_recovery(),inet_server_addr() FROM pg_stat_activity GROUP BY state;", pgConnection, pgTransaction))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    Console.WriteLine(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4));
                                }
                            }
                        }
                            
                        
                    }
                    finally
                    {
                        if (!pgTransaction.IsCompleted)
                            pgTransaction.Rollback();
                    }
                }
            }
        }
    }
}
