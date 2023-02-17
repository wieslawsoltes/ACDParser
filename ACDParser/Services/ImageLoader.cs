using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;

namespace ACDParser.Services;

public class ImageLoader
{
    private static Dictionary<string, SKBitmap?> ImageCache = new();

    public class RGB
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }

    public static void ReadBmp(string basePath, string fileName)
    {
        var imagePath = GetImagePath(basePath, fileName);
        if (!File.Exists(imagePath))
        {
            return;
        }

        // Read the BMP file
        using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);

        // Read the header
        var bfType = br.ReadUInt16();
        var bfSize = br.ReadInt32();
        var bfReserved1 = br.ReadUInt16();
        var bfReserved2 = br.ReadUInt16();
        var bfOffBits = br.ReadInt32();

        // Read the info header
        var biSize = br.ReadInt32();
        var biWidth = br.ReadInt32();
        var biHeight = br.ReadInt32();
        var biPlanes = br.ReadUInt16();
        var biBitCount = br.ReadUInt16();
        var biCompression = br.ReadInt32();
        var biSizeImage = br.ReadInt32();
        var biXPelsPerMeter = br.ReadInt32();
        var biYPelsPerMeter = br.ReadInt32();
        var biClrUsed = br.ReadInt32();
        var biClrImportant = br.ReadInt32();

        // Read the data
        var bmpData = br.ReadBytes((int)br.BaseStream.Length);

        if (biBitCount == 8)
        {
            biClrUsed = 256;
        }

        // Read the color table
        var colorTableSize = biClrUsed * 4;
        var rgb = new RGB[biClrUsed];
        if (colorTableSize > 0)
        {
            byte[] colorTable = new byte[colorTableSize];
            Array.Copy(bmpData, 0, colorTable, 0, colorTableSize);

            for (var i = 0; i < biClrUsed; i++)
            {
                rgb[i] = new RGB
                {
                    B = colorTable[i * 4],
                    G = colorTable[i * 4 + 1],
                    R = colorTable[i * 4 + 2],
                };
            }
        }
        
        // TODO: Use rgb array to get AcdCharacter.Transparency color base on index.

        // Read the pixel data
        // In case of color table those are numbers 0..255 (256 bytes total)
        var stride = (biWidth * biBitCount + 7) / 8;
        var bmpPixels = new byte[stride * biHeight];
        for (var i = 0; i < biHeight; i++)
        {
            Array.Copy(bmpData, colorTableSize + i * stride, bmpPixels, i * stride, stride);
        }
    }
    
    private static string GetImagePath(string basePath, string fileName)
    {
        var imagePath = OperatingSystem.IsWindows()
            ? $"{basePath}\\{fileName}"
            : $"{basePath}/{fileName.Replace('\\', '/')}";
        return imagePath;
    }

    public static SKBitmap? Load(string basePath, string fileName)
    {
        var imagePath = GetImagePath(basePath, fileName);
        if (!File.Exists(imagePath))
        {
            return null;
        }

        using var stream = File.Open(imagePath, FileMode.Open);
        var bitmap = SKBitmap.Decode(stream);

        // TODO: AcdCharacter.Transparency
        // TODO: AcdCharacter.ColorTable
        var pixels = bitmap.Pixels;
        var transparent = SKColor.Parse("#FF00FF");
        for (var i = 0; i < pixels.Length; i++)
        {
            var pixel = pixels[i];
            if (pixel == transparent)
            {
                pixels[i] = SKColors.Transparent;
            }
        }
        bitmap.Pixels = pixels;

        return bitmap;

    }

    public static SKBitmap? ToBitmap(string basePath, string fileName)
    {
        if (ImageCache.TryGetValue(fileName, out var bitmap))
        {
            return bitmap;
        }

        bitmap = Load(basePath, fileName);
        if (bitmap is { })
        {
            ImageCache[fileName] = bitmap;
            return bitmap;
        }

        return null;
    }
}
