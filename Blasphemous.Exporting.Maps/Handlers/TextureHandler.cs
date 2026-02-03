using UnityEngine;

namespace Blasphemous.Exporting.Maps.Handlers;

/// <summary>
/// Handles creating and destroying the textures
/// </summary>
public class TextureHandler
{
    private readonly RenderTexture _renderTexture;

    private Texture2D _layoutTexture;
    private Texture2D _hitboxTexture;

    public TextureHandler()
    {
        _renderTexture = new RenderTexture(MapExporter.WIDTH, MapExporter.HEIGHT, 24, RenderTextureFormat.ARGB32);
        _renderTexture.Create();
    }

    public void OnUnfreeze()
    {
        if (_layoutTexture != null)
            Object.Destroy(_layoutTexture);

        if (_hitboxTexture != null)
            Object.Destroy(_hitboxTexture);

        _layoutTexture = null;
        _hitboxTexture = null;
    }

    public void CreateNewTexture(int width, int height)
    {
        _layoutTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        _hitboxTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
    }

    public void ActivateRenderTexture(bool active)
    {
        RenderTexture.active = active ? _renderTexture : null;
    }

    public void AddRenderTextureToCamera(Camera camera)
    {
        camera.targetTexture = _renderTexture;
    }
}
