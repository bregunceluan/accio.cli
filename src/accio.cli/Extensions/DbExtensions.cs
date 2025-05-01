using accio.cli.Extensions;
using accio.cli.Models;
using Cocona;
using Cocona.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace accio.cli.Ebuildertensions;

public static class DbExtensions
{
    public static ICoconaCommandsBuilder MapPostgresCommands(this ICoconaCommandsBuilder builder)
    {
        builder.AddCommand("exist", async (
            Postgres postgres, 
            [Option("profile", Description = "Use profile saved")] string? profile, 
            [Option("pass", Description = "DB password")] string pass) =>
        {
            TryApplyProfile(ref postgres, profile);
            await postgres.DatabaseExist(pass);
        }).WithDescription("Checks if the database exists.");

        builder.AddCommand("listdatabases", async (Postgres postgres,
            [Option("pass", Description = "DB password")] string pass,
            [Option("profile", Description = "Use a saved profile")] string? profile
            ) =>
        {
            TryApplyProfile(ref postgres, profile);
            await postgres.ListDatabases(pass);
        }).WithDescription("Lists exists databases.");

        builder.AddCommand("create", async (
            Postgres postgres, 
            [Option("pass", Description = "DB password")] string pass,
            [Option("profile", Description = "Use a saved profile")] string? profile
            ) =>
        {
            var dbName = postgres.DataBaseName;
            TryApplyProfile(ref postgres, profile);
            postgres.DataBaseName = dbName;
            await postgres.CreateDatabase(pass);
        }).WithDescription("Creates a database with the given name.");

        builder.AddCommand("backup", async (
            Postgres postgres, 
            [Option("pass", Description = "DB password")] string pass, 
            [Option("file", Description = "Path to save the backup")] string filePath,
            [Option("profile", Description = "Use a saved profile")] string? profile
            ) =>
        {
            var dbName = postgres.DataBaseName;
            TryApplyProfile(ref postgres, profile);
            postgres.DataBaseName = dbName;
            await postgres.SaveBackup(pass, filePath);
        }).WithDescription("Creates a backup of a database.");

        builder.AddCommand("set", async (
            Postgres postgres, 
            [Option("pass", Description = "DB password")] string pass, 
            [Option("file")] string filePath, 
            [Option("force", Description = "Force is required when a change is made outside localhost.")] bool isForced,
            [Option("profile", Description = "Use a saved profile")] string? profile
            ) =>
        {
            var dbName = postgres.DataBaseName;
            TryApplyProfile(ref postgres, profile);
            postgres.DataBaseName = dbName;
            await postgres.SetFileDump(isForced, pass, filePath);
        }).WithDescription("Sets a backup file in a database.");

        builder.AddCommand("installed", async (Postgres postgres) =>
        {
            await postgres.IsPostgresInstalled();
        }).WithDescription("Checks if the psql library is installed.");

        return builder;
    }

    public static ICoconaCommandsBuilder MapProfileCommands(this ICoconaCommandsBuilder builder)
    {
        builder.AddCommand("postgres", (string name, Postgres postgres) =>
        {
            var path = AccioProfile.GetPath();

            try
            {
                Dictionary<string, AccioProfile> profiles;

                if (!File.Exists(path))
                {
                    profiles = new Dictionary<string, AccioProfile>();
                }
                else
                {
                    var json = File.ReadAllText(path);
                    profiles = JsonSerializer.Deserialize<Dictionary<string, AccioProfile>>(json)
                               ?? new Dictionary<string, AccioProfile>();
                }

                if (profiles.ContainsKey(name))
                {
                    profiles[name].Postgres = postgres;
                    Console.WriteLine($"\n✅ Perfil '{name}' successfully updated.");
                }
                else
                {
                    profiles[name] = new AccioProfile { Postgres = postgres };
                    Console.WriteLine($"\n✅ Perfil '{name}' successfully created.");
                }

                var updatedJson = JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, updatedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar/editar o perfil '{name}': {ex.Message}");
            }
        })
            .WithDescription("Create/set profile to postgres");

        builder.AddCommand("listprofiles", () =>
        {
            var profiles = AccioProfile.LoadProfiles();

            try
            {
                if (profiles == null)
                {
                    Console.WriteLine("⚠️ No profiles found. You can create one using 'accio postgres <name>'.");
                    return;
                }

                if (profiles == null || profiles.Count == 0)
                {
                    Console.WriteLine("⚠️ The profile list is empty.");
                    return;
                }

                ConsoleExtensions.Colored(ConsoleColor.Cyan, "📂 Available Profiles:");

                foreach (var profile in profiles)
                {

                    ConsoleExtensions.Colored(ConsoleColor.Yellow, $"[{profile.Key}]");
                    Console.WriteLine(JsonSerializer.Serialize(profile.Value, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
                    Console.WriteLine(new string('-', 40));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to list profiles: {ex.Message}");
            }
        })
            .WithDescription("List all profiles created.");

        builder.AddCommand("delete", 
            ([Option("profile", Description = "Use profile saved")] string profile) =>
        {
            AccioProfile.DeleteProfile(profile);
        }).WithDescription("Delete a profile");

        return builder;
    }


    static bool TryApplyProfile(ref Postgres postgres, string? profileName)
    {
        if (profileName is null)
        {
            return false;
        }

        var accioProfile = AccioProfile.TryToGetProfile(profileName);
        if (accioProfile == null)
        {
            ConsoleExtensions.Colored(ConsoleColor.Red,
                $"❌ Profile '{profileName}' not found. Use 'accio profile listprofiles' to see available profiles.");
            return false;
        }

        postgres = accioProfile.Postgres;
        ConsoleExtensions.Colored(ConsoleColor.Yellow,
            $"⚙️ Executing in profile [{profileName}]");
        return true;
    }

}

