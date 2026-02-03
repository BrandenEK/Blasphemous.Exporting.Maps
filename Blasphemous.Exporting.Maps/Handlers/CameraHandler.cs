using Blasphemous.ModdingAPI;
using UnityEngine;

namespace Blasphemous.Exporting.Maps.Handlers;

/// <summary>
/// Handles creating and moving the render camera
/// </summary>
public class CameraHandler
{
    private readonly TextureHandler _textureHandler;

    private Camera _renderCamera;

    public CameraHandler(TextureHandler textureHandler)
    {
        _textureHandler = textureHandler;
    }

    public void OnFreeze()
    {
        if (_renderCamera == null)
            CreateCamera();
    }

    private void CreateCamera()
    {
        ModLog.Info("Creating image camera");

        var obj = new GameObject("Image Camera");
        obj.transform.SetParent(Camera.main.transform, false);

        var camera = obj.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = Camera.main.orthographicSize;
        camera.aspect = Camera.main.aspect;

        _textureHandler.AddRenderTextureToCamera(camera);
        _renderCamera = camera;
    }
}
