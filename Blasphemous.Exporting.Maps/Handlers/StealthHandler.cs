using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.GameControllers.Environment;
using Gameplay.UI.Widgets;
using System.Collections.Generic;
using Tools.Level.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Exporting.Maps.Handlers;

/// <summary>
/// Handles hiding and unhiding non-desirable effects
/// </summary>
public class StealthHandler
{
    private readonly List<SpriteRenderer> _hiddenPlayerRenderers = [];

    public void OnFreeze()
    {
        // Freeze time
        ModLog.Info("Freezing time");
        SetTimeScale(0);

        // Remove fade
        var fade = Object.FindObjectOfType<FadeWidget>();
        if (fade != null)
        {
            ModLog.Info("Removing fade");
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

            _hiddenPlayerRenderers.AddRange(player.GetComponentsInChildren<SpriteRenderer>());
            foreach (var render in _hiddenPlayerRenderers)
                render.enabled = false;
        }
    }

    public void OnUnfreeze()
    {
        // Unfreeze time
        ModLog.Info("Unfreezing time");
        SetTimeScale(1);

        // Recover fade
        var fade = Object.FindObjectOfType<FadeWidget>();
        if (fade != null)
        {
            ModLog.Info("Recovering fade");
            fade.GetComponentInChildren<Image>().enabled = true;
        }

        // Recover parallax
        foreach (var parallax in Object.FindObjectsOfType<ParallaxController>())
        {
            ModLog.Info("Recovering parallax");
            for (int i = 0; i < parallax.Layers.Length; i++)
            {
                var layer = parallax.Layers[i];
                layer.layer.SetActive(true);
            }
        }

        // Show ui
        ModLog.Info("Showing UI");
        Core.UI.ShowGamePlayUIForDebug = true;

        // Show player
        var player = Core.Logic.Penitent;
        if (player != null)
        {
            ModLog.Info("Showing player");
            player.Shadow.gameObject.SetActive(true);

            foreach (var render in _hiddenPlayerRenderers)
            {
                if (render != null)
                    render.enabled = true;
            }
            _hiddenPlayerRenderers.Clear();
        }
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
