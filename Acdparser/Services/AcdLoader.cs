using System.IO;

namespace Acdparser.Services;

public class AcdLoader
{
    public static Acd? Load(string path)
    {
        using var stream = File.OpenRead(path);
        var acd = AcdParser.ParseAcd(stream);
        if (acd is { })
        {
            var basePath = Path.GetDirectoryName(path);
            if (basePath is { })
            {
                ImageLoader.BasePath = basePath;
            }

            if (acd.Character is { })
            {
                var colorTableFileName = acd.Character.ColorTable;
                if (colorTableFileName is { } && basePath is { })
                {
                    // TODO:
                    // var colorTable = ImageConverter.ToBitmap(colorTableFileName);
                    //ImageLoader.ReadBmp(basePath, colorTableFileName);
                }
            }

            //var totalAnimations = acd.Animations.Count;
            //var totalFrames = acd.Animations.SelectMany(x => x.Frames).Count();
            //var totalImages = acd.Animations.SelectMany(x => x.Frames).SelectMany(x => x.Images).Count();
            //Console.WriteLine($"animations: {totalAnimations}, frames: {totalFrames}, images: {totalImages}");

            return acd;
        }

        return null;
    }
}
