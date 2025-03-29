using accio.cli.Models;
using Cocona;

public class Program
{
    private static void Main(string[] args)
    {
        var app = CoconaApp.Create();

        app.AddSubCommand("db", x =>
        {
            x.AddCommand("exist", async (Postgres postgres, [Option("pass", Description = "DB password")] string pass) =>
            {
                await postgres.DatabaseExist(pass);
            }).WithDescription("Checks if the database exists.");

            x.AddCommand("listdatabases", async (Postgres postgres, [Option("pass", Description = "DB password")] string pass) =>
            {
                await postgres.ListDatabases(pass);
            }).WithDescription("Lists existing databases.");

            x.AddCommand("create", async (Postgres postgres, [Option("pass", Description = "DB password")] string pass) =>
            {
                await postgres.CreateDatabase(pass);
            }).WithDescription("Creates a database with the given name.");

            x.AddCommand("backup", async (Postgres postgres, [Option("pass", Description = "DB password")] string pass, [Option("file", Description = "Path to save the backup")] string filePath) =>
            {
                await postgres.SaveBackup(pass, filePath);
            }).WithDescription("Creates a backup of a database.");

            x.AddCommand("set", async (Postgres postgres, [Option("pass", Description = "DB password")] string pass, [Option("file")] string filePath, [Option("force", Description = "Force is required when a change is made outside localhost.")] bool isForced) =>
            {
                await postgres.SetFileDump(isForced, pass, filePath);
            }).WithDescription("Sets a backup file in a database.");

            x.AddCommand("installed", async (Postgres postgres) =>
            {
                await postgres.IsPostgresInstalled();
            }).WithDescription("Checks if the psql library is installed.");
        });


        app.Run();
    }

}