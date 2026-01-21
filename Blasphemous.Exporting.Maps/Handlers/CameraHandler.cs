using Blasphemous.ModdingAPI;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Exporting.Maps.Handlers;

/// <summary>
/// Handles creating and moving the render camera
/// </summary>
public class CameraHandler
{
    private Camera _renderCamera;

    public void OnFreeze()
    {
        if (_renderCamera == null)
            CreateCamera();
    }

    public void MoveCamera(Vector3 position)
    {
        _renderCamera.transform.position = Camera.main.transform.position;
    }

    private void CreateCamera()
    {
        var cameras = Object.FindObjectsOfType<Camera>();
        var camera = cameras.First(x => x.name.StartsWith("Hitbox"));
        ModLog.Info("Found hitbox camera");

        //ModLog.Info("Creating image camera");

        //var obj = new GameObject("Image Camera");
        //obj.transform.SetParent(Camera.main.transform.parent);

        //var camera = obj.AddComponent<Camera>();
        //camera.orthographic = true;
        //camera.orthographicSize = Camera.main.orthographicSize;
        //camera.aspect = Camera.main.aspect;

        Main.MapExporter.TextureHandler.AddRenderTextureToCamera(camera);
        _renderCamera = camera;
    }
}
