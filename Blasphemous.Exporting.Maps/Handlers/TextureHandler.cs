using UnityEngine;

namespace Blasphemous.Exporting.Maps.Handlers;

/// <summary>
/// Handles creating and destroying the textures
/// </summary>
public class TextureHandler
{
    private readonly RenderTexture _renderTexture;
    private Texture2D _imageTexture;

    public Texture2D ImageTexture => _imageTexture;

    public TextureHandler()
    {
        _renderTexture = new RenderTexture(MapExporter.WIDTH, MapExporter.HEIGHT, 24, RenderTextureFormat.ARGB32);
        _renderTexture.Create();
    }

    public void OnUnfreeze()
    {
        if (_imageTexture != null)
            Object.Destroy(_imageTexture);
        _imageTexture = null;
    }

    public void CreateNewTexture(int width, int height)
    {
        _imageTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
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
