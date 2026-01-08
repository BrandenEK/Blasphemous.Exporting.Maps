using Blasphemous.ModdingAPI;
using Framework.Managers;
using HarmonyLib;
using Tools.Level.Actionables;
using Tools.Level.Interactables;
using UnityEngine;

namespace Blasphemous.Exporting.Maps;

[HarmonyPatch(typeof(PersistentManager), nameof(PersistentManager.LoadSnapShot))]
class PersistentManager_LoadSnapShot_Patch
{
    public static void Postfix() => Main.MapExporter.OnLoadRoom();
}

[HarmonyPatch(typeof(BreakableWall), nameof(BreakableWall.SetCurrentPersistentState))]
class t
{
    public static void Postfix()
    {
        ModLog.Error($"BreakableWall set pers state: {Time.frameCount}");
    }
}

[HarmonyPatch(typeof(PersistentBreakableObject), nameof(PersistentBreakableObject.SetCurrentPersistentState))]
class th
{
    public static void Postfix()
    {
        ModLog.Error($"PersistentBreakableObject set pers state: {Time.frameCount}");
    }
}

[HarmonyPatch(typeof(PrieDieu), nameof(PrieDieu.SetCurrentPersistentState))]
class ts
{
    public static void Postfix()
    {
        ModLog.Error($"PrieDieu set pers state: {Time.frameCount}");
    }
}
