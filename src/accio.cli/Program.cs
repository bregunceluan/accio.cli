using accio.cli.Ebuildertensions;
using accio.cli.Models;
using Cocona;
using Cocona.Command;
using Cocona.Help.DocumentModel;
using Cocona.Help;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Cocona.Application;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


public class Program
{
    private static void Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder();

        var app = builder.Build();

        app.AddSubCommand("db", x =>
        {
            x.MapPostgresCommands();
        });

        app.AddSubCommand("profile", x =>
        {
            x.MapProfileCommands();
        });


        app.Run();


    }


}
