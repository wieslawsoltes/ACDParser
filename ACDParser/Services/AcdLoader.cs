using System.IO;
using ACDParser.Model;

namespace ACDParser.Services;

public class AcdLoader
{
    public static string BasePath = "";

    public static Acd? Load(string path)
    {
        using var stream = File.OpenRead(path);
        var acd = AcdParser.ParseStream(stream);
        if (acd is null)
        {
            return null;
        }

        var basePath = Path.GetDirectoryName(path);
        if (basePath is { })
        {
            BasePath = basePath;
        }

        if (acd.Character is { })
        {
            var colorTableFileName = acd.Character.ColorTable;
            if (colorTableFileName is { } && basePath is { })
            {
                // TODO:
                // var colorTable = ImageConverter.ToBitmap(colorTableFileName);
                // ImageLoader.ReadBmp(basePath, colorTableFileName);
            }
        }

        // var totalAnimations = acd.Animations.Count;
        // var totalFrames = acd.Animations.SelectMany(x => x.Frames).Count();
        // var totalImages = acd.Animations.SelectMany(x => x.Frames).SelectMany(x => x.Images).Count();
        // Console.WriteLine($"animations: {totalAnimations}, frames: {totalFrames}, images: {totalImages}");

        return acd;
    }
}
