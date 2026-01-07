using Blasphemous.CheatConsole;
using Blasphemous.ModdingAPI;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.Managers;
using Gameplay.GameControllers.Environment;
using Gameplay.UI.Widgets;
using System.IO;
using Tools.Level.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Exporting.Maps;

public class MapExporter : BlasMod
{
    internal MapExporter() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    public RoomStorage RoomStorage { get; private set; }

    private bool _freezeNextRoom = false;
    private bool _isFrozen = false;
    private Vector2 _cameraLocation;
    private Vector4 _cameraBounds;

    private Camera _imageCamera;
    private Texture2D _bigTex;
    private RenderTexture _renderTex;

    public void SetupNextRoom(Vector4 bounds)
    {
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        float imageHeight = (bounds.w - bounds.z + cameraHeight) * PIXEL_SCALING;
        float imageWidth = (bounds.y - bounds.x + cameraWidth) * PIXEL_SCALING;

        if (imageWidth < WIDTH)
            throw new System.Exception($"Invalid width for image: {imageWidth}px");
        if (imageHeight < HEIGHT)
            throw new System.Exception($"Invalid height for image: {imageHeight}px");

        ModLog.Warn($"Creating {(int)imageWidth}x{(int)imageHeight} texture");
        _bigTex = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.ARGB32, false);

        _freezeNextRoom = true;
        _cameraBounds = bounds;
    }

    public void StartExport(RoomInfo room)
    {
        ModLog.Info($"Starting export for room {room.Name}");

        var bounds = new Vector4(room.Xmin, room.Xmax, room.Ymin, room.Ymax);
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        float imageHeight = (bounds.w - bounds.z + cameraHeight) * PIXEL_SCALING;
        float imageWidth = (bounds.y - bounds.x + cameraWidth) * PIXEL_SCALING;

        if (imageWidth < WIDTH)
            throw new System.Exception($"Invalid width for image: {imageWidth}px");
        if (imageHeight < HEIGHT)
            throw new System.Exception($"Invalid height for image: {imageHeight}px");

        ModLog.Warn($"Creating {(int)imageWidth}x{(int)imageHeight} texture");
        _bigTex = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.ARGB32, false);

        _freezeNextRoom = true;
        _cameraBounds = bounds;

        Core.SpawnManager.SpawnFromDoor(room.Name, room.Door, true);
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

    private void PerformScreenshot()
    {
        ModLog.Info("Saving screenshot");
        RenderTexture.active = _renderTex;

        var tex = new Texture2D(WIDTH, HEIGHT, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);
        tex.Apply();

        var location = new Vector2((_cameraLocation.x - _cameraBounds.x) * PIXEL_SCALING, (_cameraLocation.y - _cameraBounds.z) * PIXEL_SCALING);
        Graphics.CopyTexture(tex, 0, 0, 0, 0, WIDTH, HEIGHT, _bigTex, 0, 0, (int)location.x, (int)location.y);

        byte[] bytes = _bigTex.EncodeToPNG();
        string path = Path.Combine(FileHandler.ContentFolder, $"{Core.LevelManager.currentLevel.LevelName}.png");

        File.WriteAllBytes(path, bytes);

        RenderTexture.active = null;
        Object.Destroy(tex);
    }

    private void PerformUnfreeze()
    {
        ModLog.Info("Unfreezing time");
        SetTimeScale(1);
        _isFrozen = false;

        if (_bigTex != null)
            Object.Destroy(_bigTex);
        _bigTex = null;
    }

    private void CreateCamera()
    {
        ModLog.Info("Creating image camera");

        var obj = new GameObject("Image Camera");
        obj.transform.SetParent(Camera.main.transform.parent);

        var camera = obj.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = Camera.main.orthographicSize;
        camera.aspect = Camera.main.aspect;
        camera.targetTexture = _renderTex;

        _imageCamera = camera;
    }

    protected override void OnInitialize()
    {
        // Setup managers
        RoomStorage = new RoomStorage(FileHandler);

        // Create render texture
        _renderTex = new RenderTexture(WIDTH, HEIGHT, 24, RenderTextureFormat.ARGB32);
        _renderTex.Create();
    }

    protected override void OnDispose()
    {
        RoomStorage.SaveRooms();
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

        // Create image camera
        if (_imageCamera == null)
            CreateCamera();

        _cameraLocation = new Vector2(_cameraBounds.x, _cameraBounds.z);
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ModLog.Warn($"Camera: {Camera.main.transform.position}");
        }
    }

    protected override void OnLateUpdate()
    {
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
        _imageCamera.transform.position = Camera.main.transform.position;

        // Handle screenshot
        if (Input.GetKeyDown(KeyCode.Alpha7))
            PerformScreenshot();

        // Handle unfreeze
        if (Input.GetKeyDown(KeyCode.Alpha0))
            PerformUnfreeze();
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new MapCommand());
    }

    private const float PARALLAX_CUTOFF = 0.3f;
    private const float CAMERA_SPEED = 30f;
    private const int WIDTH = 1920;
    private const int HEIGHT = 1080;
    private const int PIXEL_SCALING = 32 * 3;
}
