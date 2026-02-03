using Blasphemous.ModdingAPI.Files;
using Framework.Managers;
using System.IO;
using UnityEngine;

namespace Blasphemous.Exporting.Maps.Handlers;

/// <summary>
/// Handles exporting the images to a file
/// </summary>
public class ExportHandler
{
    private readonly FileHandler _fileHandler;
    private readonly TextureHandler _textureHandler;

    public ExportHandler(FileHandler fileHandler, TextureHandler textureHandler)
    {
        _fileHandler = fileHandler;
        _textureHandler = textureHandler;
    }

    public void OnScreenshot(int x, int y)
    {
        _textureHandler.ActivateRenderTexture(true);

        Texture2D fullTexture = _textureHandler.ImageTexture;
        Texture2D partTexture = new Texture2D(WIDTH, HEIGHT, TextureFormat.ARGB32, false);
        partTexture.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);
        partTexture.Apply();

        Graphics.CopyTexture(partTexture, 0, 0, 0, 0, WIDTH, HEIGHT, fullTexture, 0, 0, x, y);

        byte[] bytes = fullTexture.EncodeToPNG();
        string folder = Path.Combine(_fileHandler.ContentFolder, $"v{EXPORT_VERSION}");

        Directory.CreateDirectory(folder);
        File.WriteAllBytes(Path.Combine(folder, $"{Core.LevelManager.currentLevel.LevelName}.png"), bytes);

        _textureHandler.ActivateRenderTexture(false);
        Object.Destroy(partTexture);

    }
}
