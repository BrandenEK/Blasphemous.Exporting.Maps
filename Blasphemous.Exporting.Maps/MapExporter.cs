using Blasphemous.ModdingAPI;

namespace Blasphemous.Exporting.Maps;

public class MapExporter : BlasMod
{
    internal MapExporter() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        // Perform initialization here
    }
}
