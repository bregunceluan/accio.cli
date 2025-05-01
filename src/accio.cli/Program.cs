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

[SampleTransformHelpAttribute]
class Mes
{

}

class ApplicationMetadataProvider : ICoconaApplicationMetadataProvider
{

    public string GetDescription() => "Eru CLI";

    public string GetExecutableName() => "EruCLI";

    public string GetProductName() => "Eru Cli";

    public string GetVersion() => "1.0.0.0";
}


class SampleTransformHelpAttribute : TransformHelpAttribute
{
    public override void TransformHelp(HelpMessage helpMessage, CommandDescriptor command)
    {
        var descSection = (HelpSection)helpMessage.Children.First(x => x is HelpSection section && section.Id == HelpSectionId.Description);
        descSection.Children.Add(new HelpPreformattedText(@"
  ________ 
 < Hello! >
  -------- 
         \   ^__^
          \  (oo)\_______
             (__)\       )\/\
                 ||----w |
                 ||     ||
"));

        helpMessage.Children.Add(new HelpSection(
            new HelpHeading("Example:"),
            new HelpSection(
                new HelpParagraph("MyApp --foo --bar")
            )
        ));
    }
}