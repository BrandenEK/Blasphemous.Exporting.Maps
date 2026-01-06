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
    private bool _isFrozen = false;
    private Vector2 _cameraLocation;
    private Vector4 _cameraBounds;

    public void SetupNextRoom(Vector4 bounds)
    {
        _freezeNextRoom = true;
        _cameraBounds = bounds;
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

        // Freeze time
        ModLog.Info("Freezing time");
        SetTimeScale(0);
        _freezeNextRoom = false;
        _isFrozen = true;

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

        _cameraLocation = new Vector2(_cameraBounds.x, _cameraBounds.z);
    }

    protected override void OnLateUpdate()
    {
        if (_isFrozen && Input.GetKeyDown(KeyCode.Alpha0))
        {
            ModLog.Info("Unfreezing time");
            SetTimeScale(1);
            _isFrozen = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ModLog.Warn($"Camera: {Camera.main.transform.position}");
        }

        if (!_isFrozen)
            return;

        // Handle input
        if (Input.GetKey(KeyCode.LeftArrow))
            _cameraLocation += Vector2.left * Time.unscaledDeltaTime * CAMERA_SPEED;
        if (Input.GetKey(KeyCode.RightArrow))
            _cameraLocation += Vector2.right * Time.unscaledDeltaTime * CAMERA_SPEED;
        if (Input.GetKey(KeyCode.UpArrow))
            _cameraLocation += Vector2.up * Time.unscaledDeltaTime * CAMERA_SPEED;
        if (Input.GetKey(KeyCode.DownArrow))
            _cameraLocation += Vector2.down * Time.unscaledDeltaTime * CAMERA_SPEED;

        // Clamp camera to bounds
        if (_cameraLocation.x < _cameraBounds.x)
            _cameraLocation.x = _cameraBounds.x;
        if (_cameraLocation.x > _cameraBounds.y)
            _cameraLocation.x = _cameraBounds.y;
        if (_cameraLocation.y < _cameraBounds.z)
            _cameraLocation.y = _cameraBounds.z;
        if (_cameraLocation.y > _cameraBounds.w)
            _cameraLocation.y = _cameraBounds.w;

        // Update camera position
        Camera.main.GetComponent<ProCamera2D>().MoveCameraInstantlyToPosition(_cameraLocation);
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new MapCommand());
    }

    private const float PARALLAX_CUTOFF = 0.3f;
    private const float CAMERA_SPEED = 30f;
}
