using Blasphemous.CheatConsole;
using System;
using System.Collections.Generic;

namespace Blasphemous.Exporting.Maps;

public class MapCommand : ModCommand
{
    protected override string CommandName { get; } = "mapexport";

    protected override bool AllowUppercase { get; } = false;

    protected override Dictionary<string, Action<string[]>> AddSubCommands()
    {
        return new Dictionary<string, Action<string[]>>()
        {
            { "setup", Setup },
        };
    }

    private void Setup(string[] parameters)
    {
        Write("Setting up next room for map export");
        Main.MapExporter.SetupNextRoom();
    }
}
