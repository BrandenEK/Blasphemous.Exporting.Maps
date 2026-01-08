using Blasphemous.CheatConsole;
using Blasphemous.Exporting.Maps.Handlers;
using Blasphemous.ModdingAPI;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.Managers;
using Gameplay.UI;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Blasphemous.Exporting.Maps;

public class MapExporter : BlasMod
{
    internal MapExporter() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    public CameraHandler CameraHandler { get; private set; }
    public RoomStorage RoomStorage { get; private set; }
    public StealthHandler StealthHandler { get; private set; }
    public TextureHandler TextureHandler { get; private set; }

    private bool _freezeNextRoom = false;
    private bool _isFrozen = false;
    private bool _useDelay;
    private Vector2 _cameraLocation;
    private Vector4 _cameraBounds;

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
        TextureHandler.CreateNewTexture((int)imageWidth, (int)imageHeight);

        _freezeNextRoom = true;
        _cameraBounds = bounds;
        _useDelay = room.Delay;

        Core.SpawnManager.SpawnFromDoor(room.Name, room.Door, true);
    }

    private void PerformScreenshot()
    {
        ModLog.Info("Saving screenshot");
        TextureHandler.ActivateRenderTexture(true);

        Texture2D fullTexture = TextureHandler.ImageTexture;
        Texture2D partTexture = new Texture2D(WIDTH, HEIGHT, TextureFormat.ARGB32, false);
        partTexture.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);
        partTexture.Apply();

        var location = new Vector2((_cameraLocation.x - _cameraBounds.x) * PIXEL_SCALING, (_cameraLocation.y - _cameraBounds.z) * PIXEL_SCALING);
        Graphics.CopyTexture(partTexture, 0, 0, 0, 0, WIDTH, HEIGHT, fullTexture, 0, 0, (int)location.x, (int)location.y);

        byte[] bytes = fullTexture.EncodeToPNG();
        string path = Path.Combine(FileHandler.ContentFolder, $"{Core.LevelManager.currentLevel.LevelName}-v{EXPORT_VERSION}.png");

        File.WriteAllBytes(path, bytes);

        TextureHandler.ActivateRenderTexture(false);
        Object.Destroy(partTexture);
    }

    private void PerformFreeze()
    {
        ModLog.Info("Entering screenshot mode");
        Core.Input.SetBlocker("MAPEXPORT", true);

        _freezeNextRoom = false;
        _isFrozen = true;

        _cameraLocation = new Vector2(_cameraBounds.x, _cameraBounds.z);

        CameraHandler.OnFreeze();
        StealthHandler.OnFreeze();
    }

    private void PerformUnfreeze()
    {
        ModLog.Info("Exiting screenshot mode");
        Core.Input.SetBlocker("MAPEXPORT", false);

        _isFrozen = false;

        StealthHandler.OnUnfreeze();
        TextureHandler.OnUnfreeze();
    }

    private IEnumerator WaitForScreenshot()
    {
        if (_useDelay)
        {
            int frames = Application.targetFrameRate / DELAY_DIVISOR;
            for (int i = 0; i < frames; i++)
                yield return null;
        }
        
        PerformFreeze();
    }

    protected override void OnInitialize()
    {
        CameraHandler = new CameraHandler();
        RoomStorage = new RoomStorage(FileHandler);
        StealthHandler = new StealthHandler();
        TextureHandler = new TextureHandler();

        InputHandler.RegisterDefaultKeybindings(new System.Collections.Generic.Dictionary<string, KeyCode>()
        {
            { "TakeScreenshot", KeyCode.Minus },
            { "Exit", KeyCode.Equals },
        });
    }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        if (!_freezeNextRoom)
            return;

        UIController.instance.StartCoroutine(WaitForScreenshot());
    }

    protected override void OnDispose()
    {
        RoomStorage.SaveRooms();
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
        CameraHandler.MoveCamera(Camera.main.transform.position);

        // Handle screenshot
        if (InputHandler.GetKeyDown("TakeScreenshot"))
            PerformScreenshot();

        // Handle unfreeze
        if (InputHandler.GetKeyDown("Exit"))
            PerformUnfreeze();
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new MapCommand());
    }

    internal const int WIDTH = 640;
    internal const int HEIGHT = 360;
    private const int PIXEL_SCALING = 32;
    private const float CAMERA_SPEED = 30f;
    private const int DELAY_DIVISOR = 5;
    private const int EXPORT_VERSION = 2;
}
