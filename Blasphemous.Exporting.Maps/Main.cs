using BepInEx;

namespace Blasphemous.Exporting.Maps;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "0.1.0")]
internal class Main : BaseUnityPlugin
{
    public static Maps Maps { get; private set; }

    private void Start()
    {
        Maps = new Maps();
    }
}
