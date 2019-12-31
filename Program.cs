using System;
using System.Diagnostics;
using Npgsql;

namespace npgsqlTester
{
    class Program
    {
        /*
        static void Main(string[] args)
        {
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;Pooling=false;";                     //works, pg1 without failover, due to implicity TargetServerType=Any
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Any";             //works, pg1 without failover
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Primary;";        //works, straight to pg1
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Secondary;";    //works, tests and fails over to pg2
            //var connectionString = "Host=PG2,PG1;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Secondary;";    //works, straight to pg2
            //var connectionString = "Host=PG2;username=robtest_login;password=robtest_password;Database=robtest;";
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Primary;Pooling=true;KeepAlive=5;";        //works, straight to pg1
            var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Primary;Pooling=true;KeepAlive=5;Enlist=false;Minimum Pool Size=4;";        //works, straight to pg1

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
                
            } while (true || Console.ReadLine() != "q");        
        }
        */
        static void Main(string[] args)
        {
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;Pooling=false;";                     //works, pg1 without failover, due to implicity TargetServerType=Any
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Any";             //works, pg1 without failover
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Primary;";        //works, straight to pg1
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Secondary;";    //works, tests and fails over to pg2
            //var connectionString = "Host=PG2,PG1;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Secondary;";    //works, straight to pg2
            //var connectionString = "Host=PG2;username=robtest_login;password=robtest_password;Database=robtest;";
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Primary;Pooling=true;KeepAlive=5;";        //works, straight to pg1
            var primaryConnectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Primary;Pooling=true;KeepAlive=500;Enlist=false;Minimum Pool Size=4;";
            var secondaryConnectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetServerType=Secondary;Pooling=true;KeepAlive=500;Enlist=false;Minimum Pool Size=4;"; 

            do
            {
                try
                {
                    Console.WriteLine("-------------------------------------");
                    using (var primaryConnection = new NpgsqlConnection(primaryConnectionString))
                    using (var secondaryConnection = new NpgsqlConnection(secondaryConnectionString))
                    {
                        primaryConnection.Open();
                        secondaryConnection.Open();
                        
                        using (var pgPrimaryTransaction = primaryConnection.BeginTransaction())
                        using (var pgSecondaryTransaction = secondaryConnection.BeginTransaction())
                        {
                            try
                            {
                                using (var primaryCommand = new NpgsqlCommand("SELECT state,COUNT(1),pg_is_in_recovery(),inet_server_addr()::text FROM pg_stat_activity GROUP BY state;", primaryConnection, pgPrimaryTransaction))
                                {
                                    using (var reader = primaryCommand.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Console.WriteLine("primary {0} {1} {2} {3}", reader.GetString(0),reader.GetInt64(1), reader.GetBoolean(2), reader.GetString(3));
                                        }
                                    }
                                }
                        
                                using (var secondaryCommand = new NpgsqlCommand("SELECT state,COUNT(1),pg_is_in_recovery(),inet_server_addr()::text FROM pg_stat_activity GROUP BY state;", secondaryConnection, pgSecondaryTransaction))
                                {
                                    using (var reader = secondaryCommand.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Console.WriteLine("seconda {0} {1} {2} {3}", reader.GetString(0), reader.GetInt64(1), reader.GetBoolean(2), reader.GetString(3));
                                        }
                                    }
                                }
                        
                            }
                            finally
                            {
                                pgPrimaryTransaction.Rollback();
                                pgSecondaryTransaction.Rollback();
                            }
                        }
                        Console.WriteLine("q to quit, anything else to run query again!");
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    Debugger.Break();
                }

            } while (true || Console.ReadLine() != "q");
        }
    }
}
