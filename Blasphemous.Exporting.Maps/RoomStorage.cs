using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Exporting.Maps;

public class RoomStorage
{
    private readonly Dictionary<string, RoomInfo> _rooms;

    public RoomStorage(FileHandler file)
    {
        if (!file.LoadDataAsJson("rooms.json", out RoomInfo[] infos))
        {
            ModLog.Error($"Failed to load room info");
            return;
        }

        _rooms = infos.ToDictionary(x => x.Name, x => x);
        ModLog.Info($"Loaded {_rooms.Count} rooms");
    }

    public bool DoesRoomExist(string room)
    {
        return _rooms.ContainsKey(room);
    }

    public bool TryGetRoom(string room, out RoomInfo result)
    {
        return _rooms.TryGetValue(room, out result);
    }

    private RoomInfo CreateRoom(string room)
    {
        var info = new RoomInfo()
        {
            Name = room,
            Door = string.Empty,
        };

        _rooms.Add(room, info);
        return info;
    }

    private RoomInfo CreateIfDoesntExist(string room)
    {
        if (_rooms.TryGetValue(room, out RoomInfo info))
            return info;

        return CreateRoom(room);
    }

    public void UpdateDoor(string room, string door)
    {
        RoomInfo info = CreateIfDoesntExist(room);

        info.Door = door;

        // Save
    }

    public void UpdateMinBounds(string room, Vector2 position)
    {
        RoomInfo info = CreateIfDoesntExist(room);

        info.Xmin = position.x;
        info.Ymin = position.y;

        // Save
    }

    public void UpdateMaxBounds(string room, Vector2 position)
    {
        RoomInfo info = CreateIfDoesntExist(room);

        info.Ymin = position.x;
        info.Ymax = position.y;

        // Save
    }
}
