using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Threading;
using SkiaSharp;

namespace Acdparser;

internal class BitmapFrame
{
    public BitmapFrame(SKBitmap bitmap, TimeSpan duration)
    {
        Bitmap = bitmap;
        Duration = duration;
    }

    public SKBitmap Bitmap { get; set; }

    public TimeSpan Duration { get; set; }
}

public partial class AnimationView : UserControl
{
    public AnimationView()
    {
        InitializeComponent();
    }

    private List<BitmapFrame>? _frames;
    private int _lastFrameIndex = -1;
    private BitmapFrame? _lastFrame;
    private IDisposable? _dispose;

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        Unload();

        if (DataContext is { })
        {
            Load();
        }
    }

    private void Load()
    {
        if (DataContext is DefineAnimation animation)
        {
            Console.WriteLine($"OnLoaded {animation.Name}");
            
            _frames = new List<BitmapFrame>();
            for (var i = 0; i < animation.Frames.Count; i++)
            {
                var frame = animation.Frames[i];
                if (frame.Images.Count >= 1)
                {
                    var branching = frame.Branching;
                    var exitBranch = frame.ExitBranch;
                    // TODO: Handle branching and exit branch.

                    var image = frame.Images[0];
                    if (image.Filename is { })
                    {
                        var bitmap = ImageConverter.ToBitmap(image.Filename);
                        if (bitmap is { })
                        {
                            var duration = TimeSpan.FromMilliseconds(frame.Duration * 10);
                            var bitmapFrame = new BitmapFrame(bitmap, duration);
                            _frames.Add(bitmapFrame);
                        }
                    }
                }
            }

            if (_frames is { } && _frames.Count >= 0)
            {
                _lastFrameIndex = 0;
                _lastFrame = _frames[_lastFrameIndex];
                RenderFrame(_lastFrame);
            }
        }
    }

    private void Unload()
    {
        if (DataContext is DefineAnimation animation)
        {
            Console.WriteLine($"OnUnloaded {animation.Name}");
        }

        _frames = null;
        _lastFrame = null;
        _lastFrameIndex = -1;
        _dispose?.Dispose();
        _dispose = null;
    }

    private void RenderFrame(BitmapFrame bitmapFrame)
    {
        _dispose = DispatcherTimer.RunOnce(
            () =>
            {
                if (_frames is null)
                {
                    return;
                }
                BitmapControl.Bitmap = bitmapFrame.Bitmap;
                _lastFrameIndex++;
                if (_lastFrameIndex >= _frames.Count)
                {
                    _lastFrameIndex = 0;
                }
                _lastFrame = _frames[_lastFrameIndex];
                RenderFrame(_lastFrame);
            },
            bitmapFrame.Duration,
            DispatcherPriority.Render);
    }
}
