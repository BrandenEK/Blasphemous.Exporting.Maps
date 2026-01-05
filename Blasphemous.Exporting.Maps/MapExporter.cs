using Blasphemous.CheatConsole;
using Blasphemous.ModdingAPI;

namespace Blasphemous.Exporting.Maps;

public class MapExporter : BlasMod
{
    internal MapExporter() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private bool _freezeNextRoom = false;

    protected override void OnInitialize()
    {
        // Perform initialization here
    }

    public void SetupNextRoom()
    {
        _freezeNextRoom = true;
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new MapCommand());
    }
}
