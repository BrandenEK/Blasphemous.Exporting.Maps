using Blasphemous.CheatConsole;
using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.GameControllers.Environment;
using Gameplay.UI.Widgets;
using Tools.Level.Layout;
using UnityEngine;
using UnityEngine.UI;

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

    private void SetTimeScale(float time)
    {
        Time.timeScale = time;

        var obj = Object.FindObjectOfType<LevelInitializer>();
        if (obj != null)
        {
            obj.TimeScaleReal = time;
        }
    }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        if (!_freezeNextRoom)
            return;
        _freezeNextRoom = false;

        ModLog.Info("load stop");

        // Freeze time
        ModLog.Info("Freezing time");
        SetTimeScale(0);

        // Hide fade
        var fade = Object.FindObjectOfType<FadeWidget>();
        if (fade != null)
        {
            ModLog.Warn("Clearing fade");
            //fade.ClearFade();
            //foreach (Transform t in fade.transform)
            //    ModLog.Error(t.name);

            fade.GetComponentInChildren<Image>().enabled = false;
            _camPos = Camera.main.transform.position;
        }

        // Hide parallax
        var list = Object.FindObjectsOfType<ParallaxController>();
        //var parallax = Object.FindObjectOfType<ParallaxController>();
        //if (parallax != null)
        //{
        //}

        foreach (var parallax in list)
        {
            ModLog.Warn("Removing parallax");
            for (int i = 0; i < parallax.Layers.Length; i++)
            {
                var layer = parallax.Layers[i];
                ModLog.Error($"Layer {layer.layer.name}: {layer.speed}");

                //float speed = 0;
                //GameObject obj = null;

                //if (layer.speed >= 0.85f)
                //{
                //    speed = 1;
                //    obj = layer.layer;
                //}
                //else if (Mathf.Abs(layer.speed) <= 0.15f)
                //{
                //    speed = 0;
                //    obj = layer.layer;
                //}
                //else
                //{
                //    layer.layer.SetActive(false);
                //}

                //parallax.Layers[i] = new ParallaxData()
                //    {
                //        layer = layer.layer,
                //        speed = 0,
                //    };

                //if (layer.speed >= 0.9)
                //{
                //    parallax.Layers[i] = new ParallaxData()
                //    {
                //        layer = layer.layer,
                //        speed = 1,
                //    };
                //}
                //else if (layer.speed != 1 || layer.speed != 0)
                //    layer.layer.SetActive(false);

                //layer.layer = null;

                //layer.speed = 0;

                //parallax.Layers[i] = new ParallaxData()
                //{
                //    layer = layer.layer,
                //    speed = 1
                //};

                if (Mathf.Abs(layer.speed) <= 0.3f)
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
        Core.UI.ShowGamePlayUIForDebug = false;

        // Hide player
        var player = Core.Logic.Penitent;
        if (player != null)
        {
            ModLog.Warn("Hiding player");
            foreach (var render in player.GetComponentsInChildren<SpriteRenderer>())
                render.enabled = false;

            player.Shadow.gameObject.SetActive(false);
        }
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new MapCommand());
    }
}
