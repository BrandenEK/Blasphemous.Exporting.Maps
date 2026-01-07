using Framework.Managers;
using HarmonyLib;

namespace Blasphemous.Exporting.Maps;

[HarmonyPatch(typeof(PersistentManager), nameof(PersistentManager.LoadSnapShot))]
class PersistentManager_LoadSnapShot_Patch
{
    public static void Postfix() => Main.MapExporter.OnLoadRoom();
}
