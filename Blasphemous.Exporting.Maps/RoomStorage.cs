using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
using System.Collections.Generic;
using System.Linq;

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
}
