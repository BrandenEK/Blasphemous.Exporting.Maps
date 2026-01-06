using Blasphemous.CheatConsole;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        if (!ValidateParameterList(parameters, 4))
            return;

        if (!ValidateFloatParameter(parameters[0], float.MinValue, float.MaxValue, out float xmin))
            return;
        if (!ValidateFloatParameter(parameters[1], float.MinValue, float.MaxValue, out float xmax))
            return;
        if (!ValidateFloatParameter(parameters[2], float.MinValue, float.MaxValue, out float ymin))
            return;
        if (!ValidateFloatParameter(parameters[3], float.MinValue, float.MaxValue, out float ymax))
            return;

        Main.MapExporter.SetupNextRoom(new Vector4(xmin, xmax, ymin, ymax));
    }
}
