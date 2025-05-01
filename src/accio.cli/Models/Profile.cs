using accio.cli.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace accio.cli.Models;


public class AccioConfig
{
    public Dictionary<string, AccioProfile> Profiles { get; set; } = new();
}

public class AccioProfile
{

    public Postgres Postgres { get; set; } = new Postgres();

    public override string? ToString()
    {
        return $"Postgres: {Postgres.ToString()}";
    }

    public static string GetPath()
    {
        const string relativePath = "Configs/config.json";

        //string codeBase = Assembly.GetExecutingAssembly().;
        //UriBuilder uri = new UriBuilder(codeBase);
        //string assemblyPath = Uri.UnescapeDataString(uri.Path);

        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        return path;
    }

    public static Dictionary<string, AccioProfile>? LoadProfiles(bool createIt = false)
    {
        var path = GetPath();

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {

            if (!createIt)
            {
                return null;
            }

            File.WriteAllText(path, "");
        }

        var json = File.ReadAllText(path);

        var profiles = JsonSerializer.Deserialize<Dictionary<string, AccioProfile>>(json)
                               ?? new Dictionary<string, AccioProfile>();

        return profiles;
    }

    public static AccioProfile? TryToGetProfile(string profileName)
    {
        var profiles = LoadProfiles();

        if (profiles == null)
        {
            return null;
        }

        profiles.TryGetValue(profileName, out var profile);
        return profile;

    }

    public static void DeleteProfile(string profileName)
    {
        var path = GetPath();

        var profile = TryToGetProfile(profileName);

        if (profile == null)
        {
            ConsoleExtensions.Colored(ConsoleColor.Yellow, $"Profile '{profileName}' not found.");
            return;

        }


        var profiles = LoadProfiles();

        profiles.Remove(profileName);

        var json = JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(path, json);

        ConsoleExtensions.Colored(ConsoleColor.Yellow, $"Profile '{profileName}' deleted.");
        return;


    }

}

