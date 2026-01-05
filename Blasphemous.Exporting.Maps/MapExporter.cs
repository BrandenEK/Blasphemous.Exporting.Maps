using Blasphemous.CheatConsole;
using Blasphemous.ModdingAPI;
using Com.LuisPedroFonseca.ProCamera2D;
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
    private Vector2 _cameraLocation;

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
                ModLog.Error($"Layer {layer.layer.name}: {layer.speed}");

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

        _cameraLocation = Camera.main.transform.position;
    }

    protected override void OnLateUpdate()
    {
        // Lock camera within certain bounds

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ModLog.Warn("Move right");
            _cameraLocation += Vector2.right * CAMERA_SPEED;
            //Camera.main.GetComponent<ProCamera2D>().MoveCameraInstantlyToPosition(_cameraLocation);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ModLog.Warn("Move left");
            _cameraLocation += Vector2.left * CAMERA_SPEED;
            //Camera.main.GetComponent<ProCamera2D>().MoveCameraInstantlyToPosition(_cameraLocation);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ModLog.Warn("Move up");
            _cameraLocation += Vector2.up * CAMERA_SPEED;
            //Camera.main.GetComponent<ProCamera2D>().MoveCameraInstantlyToPosition(_cameraLocation);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ModLog.Warn("Move down");
            _cameraLocation += Vector2.down * CAMERA_SPEED;
            //Camera.main.GetComponent<ProCamera2D>().MoveCameraInstantlyToPosition(_cameraLocation);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ModLog.Info("Unfreezing time");
            SetTimeScale(1);

            //foreach (var comp in Camera.main.GetComponents<Component>())
            //{
            //    ModLog.Info(comp.GetType().Name);
            //}
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ModLog.Warn($"Camera: {Camera.main.transform.position}");
        }

        // Clamp camera to bounds
        Camera.main.GetComponent<ProCamera2D>().MoveCameraInstantlyToPosition(_cameraLocation);
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new MapCommand());
    }

    private const float PARALLAX_CUTOFF = 0.3f;
    private const float CAMERA_SPEED = 2f;
}
