using System;
using Npgsql;

namespace npgsqlTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;Pooling=false;";
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;";
            //var connectionString = "Host=PG1;username=robtest_login;password=robtest_password;Database=robtest;";

            do
            {
                try
                {
                    using (var pgConnection = new NpgsqlConnection(connectionString))
                    {
                        pgConnection.Open();
                        using (var pgTransaction = pgConnection.BeginTransaction())
                        {
                            try
                            {
                                using (var command = new NpgsqlCommand("SELECT state,COUNT(1),pg_is_in_recovery(),inet_server_addr()::text FROM pg_stat_activity GROUP BY state;", pgConnection, pgTransaction))
                                {
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Console.WriteLine("{0} {1} {2} {3}", reader.GetString(0), reader.GetInt64(1), reader.GetBoolean(2), reader.GetString(3));
                                        }
                                    }
                                }


                            }
                            finally
                            {
                                pgTransaction.Rollback();
                            }
                        }
                    }
                    Console.WriteLine("q to quit, anything else to run query again!");
                }
                catch(Exception e)
                {
                    Console.Error.WriteLine(e);
                }
                
            } while (Console.ReadLine() != "q");


            
        }
    }
}
