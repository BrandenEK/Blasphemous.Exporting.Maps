using BepInEx;

namespace Blasphemous.Exporting.Maps;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "2.5.0")]
internal class Main : BaseUnityPlugin
{
    public static MapExporter MapExporter { get; private set; }

    private void Start()
    {
        MapExporter = new MapExporter();
    }
}
