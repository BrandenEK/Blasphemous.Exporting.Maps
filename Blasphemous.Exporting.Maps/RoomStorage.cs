using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Exporting.Maps;

public class RoomStorage
{
    private readonly Dictionary<string, RoomInfo> _rooms;

    public RoomStorage(FileHandler file)
    {
        _rooms = LoadFromContent(file);
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

        SaveRooms();
    }

    public void UpdateDelay(string room, bool delay)
    {
        RoomInfo info = CreateIfDoesntExist(room);
        info.Delay = delay;

        SaveRooms();
    }

    public void UpdateMinBounds(string room, Vector2 position)
    {
        RoomInfo info = CreateIfDoesntExist(room);
        info.Xmin = position.x;
        info.Ymin = position.y;

        SaveRooms();
    }

    public void UpdateMaxBounds(string room, Vector2 position)
    {
        RoomInfo info = CreateIfDoesntExist(room);
        info.Xmax = position.x;
        info.Ymax = position.y;

        SaveRooms();
    }

    public void SaveRooms()
    {
        string path = Path.Combine(Main.MapExporter.FileHandler.ContentFolder, "rooms.json");
        var settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy(false, true)
            },
        };
        string json = JsonConvert.SerializeObject(_rooms.Values.OrderBy(x => x.Name), settings);

        ModLog.Info("Saving room info to content folder");
        File.WriteAllText(path, json);
    }

    private Dictionary<string, RoomInfo> LoadFromData(FileHandler file)
    {
        if (!file.LoadDataAsJson("rooms.json", out RoomInfo[] infos))
        {
            ModLog.Error($"Failed to load room info");
            return [];
        }

        return infos.ToDictionary(x => x.Name, x => x);
    }

    private Dictionary<string, RoomInfo> LoadFromContent(FileHandler file)
    {
        try
        {
            string path = Path.Combine(file.ContentFolder, "rooms.json");
            string json = File.ReadAllText(path);

            RoomInfo[] infos = JsonConvert.DeserializeObject<RoomInfo[]>(json);
            return infos.ToDictionary(x => x.Name, x => x);
        }
        catch (System.Exception ex)
        {
            ModLog.Error($"Failed to load room info: {ex}");
            return [];
        }
    }
}
