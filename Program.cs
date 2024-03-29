﻿using System;
using System.Diagnostics;
using Npgsql;

namespace npgsqlTester
{
    class Program
    {
        /*
        static void Main(string[] args)
        {
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;Pooling=false;";                     //works, pg1 without failover, due to implicity TargetSessionAttributes=Any
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Any";             //works, pg1 without failover
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Primary;";        //works, straight to pg1
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Secondary;";    //works, tests and fails over to pg2
            //var connectionString = "Host=PG2,PG1;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Secondary;";    //works, straight to pg2
            //var connectionString = "Host=PG2;username=robtest_login;password=robtest_password;Database=robtest;";
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Primary;Pooling=true;KeepAlive=5;";        //works, straight to pg1
            var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Primary;Pooling=true;KeepAlive=5;Enlist=false;Minimum Pool Size=4;";        //works, straight to pg1

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
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;Pooling=false;";                     //works, pg1 without failover, due to implicity TargetSessionAttributes=Any
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Any";             //works, pg1 without failover
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Primary;";        //works, straight to pg1
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Secondary;";    //works, tests and fails over to pg2
            //var connectionString = "Host=PG2,PG1;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Secondary;";    //works, straight to pg2
            //var connectionString = "Host=PG2;username=robtest_login;password=robtest_password;Database=robtest;";
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Primary;Pooling=true;KeepAlive=5;";        //works, straight to pg1
            
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Primary;Pooling=false";             //
            //var connectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Primary;Pooling=false;KeepAlive=5";

            var primaryConnectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Primary;HostRecheckSeconds=1;Pooling=true;KeepAlive=5;Enlist=false;Minimum Pool Size=4;Multiplexing=false;";
            var secondaryConnectionString = "Host=PG1,PG2;username=robtest_login;password=robtest_password;Database=robtest;TargetSessionAttributes=Secondary;HostRecheckSeconds=1;Pooling=true;KeepAlive=5;Enlist=false;Minimum Pool Size=4;Multiplexing=false;";
            //var primaryConnectionString = connectionString;

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
                                using (var primaryCommand = new NpgsqlCommand("SELECT pg_is_in_recovery(),inet_server_addr()::text;", primaryConnection, pgPrimaryTransaction))
                                {
                                    using (var reader = primaryCommand.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Console.WriteLine("primary {0} {1}", reader.GetBoolean(0), reader.GetString(1));
                                        }
                                    }
                                }

                                using (var secondaryCommand = new NpgsqlCommand("SELECT pg_is_in_recovery(),inet_server_addr()::text;", secondaryConnection, pgSecondaryTransaction))
                                {
                                    using (var reader = secondaryCommand.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Console.WriteLine("seconda {0} {1}", reader.GetBoolean(0), reader.GetString(1));
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
                    //Debugger.Break();
                }

            } while (Console.ReadLine().ToLower() != "q");
        }
    }
}
