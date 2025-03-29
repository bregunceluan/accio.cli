using CliWrap;
using CliWrap.Buffered;
using Cocona;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accio.cli.Models;

public record Postgres
(
    [Option("u", Description = "Database user")]
    string UserName="postgres",

    [Option("d", Description = "Database name")]
    string DataBaseName="postgres",

    [Option("h", Description = "Database url")]
    string Host = "localhost",

    [Option("p", Description = "Database port")]
    int Port = 5432
) : ICommandParameterSet;


public static class PostgresOperations
{
    public async static Task<CommandResult> IsPostgresInstalled(this Postgres postgres)
    {
        try
        {
            var result = await Cli.Wrap("psql")

                .WithArguments("--version")
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
                .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
                .ExecuteBufferedAsync();

            if (!result.IsSuccess)
            {
                Console.WriteLine("O psql parece não estar instalado em seu computador.\nAcesse https://www.postgresql.org/download/");
                return null;
            }

            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static async Task<CommandResult> DatabaseExist(this Postgres postgres, string password)
    {
        try
        {

            var commandText = $"\"SELECT 1 FROM pg_database WHERE datname = '{postgres.DataBaseName}';\"";

            var result = await Cli.Wrap("psql")
                         .WithEnvironmentVariables((a) => { a.Set("PGPASSWORD", password); })
                         .WithArguments($"-h {postgres.Host} -d {postgres.DataBaseName} -U {postgres.UserName} -c {commandText}")
                         .WithValidation(CommandResultValidation.None)
                         .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
                         .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
                         .ExecuteBufferedAsync();

            return result;
        }
        catch (Exception)
        {

            return null;
        }
    }
    public static async Task<CommandResult> ListDatabases(this Postgres postgres, string password)
    {
        try
        {

            var result = await Cli.Wrap("psql")
                         .WithEnvironmentVariables((a) => { a.Set("PGPASSWORD", password); })
                         .WithArguments($"-h {postgres.Host} -U {postgres.UserName} -c \"SELECT datname FROM pg_database;\"")
                         .WithValidation(CommandResultValidation.None)
                         .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
                         .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
                         .ExecuteBufferedAsync();

            return result;
        }
        catch (Exception)
        {

            return null;
        }
    }
    public static async Task<CommandResult> CreateDatabase(this Postgres postgres, string password)
    {
        try
        {
            var command = $"\"CREATE DATABASE {postgres.DataBaseName}\"";

            var result = await Cli.Wrap("psql")
                         .WithEnvironmentVariables((a) => { a.Set("PGPASSWORD", password); })
                         .WithArguments($"-h {postgres.Host} -U {postgres.UserName} -c {command}")
                         .WithValidation(CommandResultValidation.None)
                         .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
                         .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
                         .ExecuteBufferedAsync();

            if (result.IsSuccess)
            {
                Console.WriteLine("Database criado com sucesso!!!");
            }

            return result;
        }
        catch (Exception)
        {

            return null;
        }
    }
    public static async Task<CommandResult> SaveBackup(this Postgres postgres, string password, string filePath)
    {
        try
        {

            var path = Path.Combine(filePath, postgres.DataBaseName + "_backup.dump");

            var result = await Cli.Wrap("pg_dump")
                         .WithEnvironmentVariables((a) => { a.Set("PGPASSWORD", password); })
                         .WithArguments($"-h {postgres.Host} -U {postgres.UserName} -d {postgres.DataBaseName} -F c -f {path}")
                         .WithValidation(CommandResultValidation.None)
                         .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
                         .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
                         .ExecuteBufferedAsync();

            if (result.IsSuccess)
            {
                Console.WriteLine("Backup criado com sucesso!!!");
            }

            return result;
        }
        catch (Exception)
        {

            return null;
        }
    }
    public static async Task<CommandResult> SetFileDump(this Postgres postgres, bool isForced, string password, string filePath)
    {
        try
        {

            if (!isForced && !postgres.Host.Contains("localhost"))
            {
                Console.WriteLine("Para setar backup de um banco que não seja em localhost, utilize --force.");
                return null;
            }

            var result = await Cli.Wrap("pg_restore")
                         .WithEnvironmentVariables((a) => { a.Set("PGPASSWORD", password); })
                         .WithArguments($"-h {postgres.Host} -U {postgres.UserName} -d {postgres.DataBaseName} -c {filePath}")
                         .WithValidation(CommandResultValidation.None)
                         .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
                         .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
                         .ExecuteBufferedAsync();

            if (result.IsSuccess)
            {
                Console.WriteLine("Backup restaurado com sucesso!!");
                Process.Start("explorer.exe", filePath);
            }


            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

}