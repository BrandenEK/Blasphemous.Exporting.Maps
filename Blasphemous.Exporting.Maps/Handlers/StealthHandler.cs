using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.GameControllers.Environment;
using Gameplay.UI.Widgets;
using Tools.Level.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Exporting.Maps.Handlers;

/// <summary>
/// Handles hiding and unhiding non-desirable effects
/// </summary>
public class StealthHandler
{
    public void OnFreeze()
    {
        // Freeze time
        ModLog.Info("Freezing time");
        SetTimeScale(0);

        // Clear fade
        var fade = Object.FindObjectOfType<FadeWidget>();
        if (fade != null)
        {
            ModLog.Info("Clearing fade");
            fade.GetComponentInChildren<Image>().enabled = false;
        }

        // Remove parallax
        foreach (var parallax in Object.FindObjectsOfType<ParallaxController>())
        {
            ModLog.Info("Removing parallax");
            for (int i = 0; i < parallax.Layers.Length; i++)
            {
                var layer = parallax.Layers[i];
                //ModLog.Error($"Layer {layer.layer.name}: {layer.speed}");

                if (Mathf.Abs(layer.speed) <= PARALLAX_CUTOFF)
                {
                    parallax.Layers[i] = new ParallaxData()
                    {
                        layer = layer.layer,
                        speed = 0,
                    };
                }
                else
                {
                    layer.layer.SetActive(false);
                }
            }

        }

        // Hide ui
        ModLog.Info("Hiding UI");
        Core.UI.ShowGamePlayUIForDebug = false;

        // Hide player
        var player = Core.Logic.Penitent;
        if (player != null)
        {
            ModLog.Info("Hiding player");
            player.Shadow.gameObject.SetActive(false);

            foreach (var render in player.GetComponentsInChildren<SpriteRenderer>())
                render.enabled = false;
        }
    }

    public void OnUnfreeze()
    {
        // Unfreeze time
        ModLog.Info("Unfreezing time");
        SetTimeScale(1);

        // Recover fade

        // Recover parallax

        // Show ui

        // Show player
    }

    private void SetTimeScale(float time)
    {
        Time.timeScale = time;

        var obj = Object.FindObjectOfType<LevelInitializer>();
        if (obj != null)
        {
            obj.TimeScaleReal = time;
        }
    }

    private const float PARALLAX_CUTOFF = 0.3f;
}
