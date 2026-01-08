using Blasphemous.CheatConsole;
using Framework.Managers;
using System.Collections.Generic;
using System.Linq;
using Tools.Level.Interactables;
using UnityEngine;

namespace Blasphemous.Exporting.Maps;

public class MapCommand : ModCommand
{
    protected override string CommandName { get; } = "me";

    protected override bool AllowUppercase { get; } = false;

    protected override Dictionary<string, System.Action<string[]>> AddSubCommands()
    {
        return new Dictionary<string, System.Action<string[]>>()
        {
            { "load", Load },
            { "door", Door },
            { "delay", Delay },
            { "min", Min },
            { "max", Max },
        };
    }

    private void Load(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 1))
            return;

        string text = parameters[0].ToUpper();

        if (Core.LevelManager.currentLevel.LevelName == text)
        {
            Write($"Can not export the currently loaded room");
            return;
        }

        if (!Main.MapExporter.RoomStorage.TryGetRoom(text, out RoomInfo room))
        {
            Write($"Room {text} does not have data");
            return;
        }

        Main.MapExporter.StartExport(room);
    }

    private void Door(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        string room = Core.LevelManager.currentLevel.LevelName;
        Vector3 playerPosition = Core.Logic.Penitent.transform.position;
        Door door = Object.FindObjectsOfType<Door>().OrderBy(x => Vector3.Distance(playerPosition, x.transform.position)).FirstOrDefault();

        if (door == null)
        {
            Write("No door could be found");
            return;
        }

        Write($"Storing door for {room} as {door.identificativeName}");
        Main.MapExporter.RoomStorage.UpdateDoor(room, door.identificativeName);
    }

    private void Delay(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 1))
            return;

        string room = Core.LevelManager.currentLevel.LevelName;
        bool delay = parameters[0] == "on";

        Write($"Storing delay state for {room} as {delay}");
        Main.MapExporter.RoomStorage.UpdateDelay(room, delay);
    }

    private void Min(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        string room = Core.LevelManager.currentLevel.LevelName;
        Vector2 position = Camera.main.transform.position;

        Write($"Storing min position for {room} as {position}");
        Main.MapExporter.RoomStorage.UpdateMinBounds(room, position);
    }

    private void Max(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        string room = Core.LevelManager.currentLevel.LevelName;
        Vector2 position = Camera.main.transform.position;

        Write($"Storing max position for {room} as {position}");
        Main.MapExporter.RoomStorage.UpdateMaxBounds(room, position);
    }
}
