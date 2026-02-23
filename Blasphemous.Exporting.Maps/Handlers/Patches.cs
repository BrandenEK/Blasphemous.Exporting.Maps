using Gameplay.GameControllers.Effects.Player.Dust;
using HarmonyLib;
using UnityEngine;

namespace Blasphemous.Exporting.Maps.Handlers;

/// <summary>
/// Hide the dust by spawning it far away
/// </summary>
[HarmonyPatch(typeof(StepDustSpawner), nameof(StepDustSpawner.GetStepDust))]
class StepDustSpawner_GetStepDust_Patch
{
    public static void Prefix(ref Vector2 stepDustPosition)
    {
        stepDustPosition = new Vector2(-1000, -1000);
    }
}
