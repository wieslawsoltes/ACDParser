using System;
using System.Collections.Generic;
using ACDParser.Model;
using ACDParser.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Skia;
using Avalonia.Threading;
using SkiaSharp;

namespace ACDParser.Controls;

public class Player : TemplatedControl
{
    public static readonly StyledProperty<AcdAnimation?> AnimationProperty = 
        AvaloniaProperty.Register<Player, AcdAnimation?>(nameof(Animation));

    public AcdAnimation? Animation
    {
        get => GetValue(AnimationProperty);
        set => SetValue(AnimationProperty, value);
    }

    private class BitmapFrame
    {
        public BitmapFrame(SKBitmap bitmap, TimeSpan duration)
        {
            Bitmap = bitmap;
            Duration = duration;
        }

        public SKBitmap Bitmap { get; set; }

        public TimeSpan Duration { get; set; }
    }

    private SKBitmapControl? _bitmapControl;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _bitmapControl = e.NameScope.Find<SKBitmapControl>("PART_BitmapControl");
    }

    private List<BitmapFrame>? _frames;
    private int _lastFrameIndex = -1;
    private BitmapFrame? _lastFrame;
    private IDisposable? _dispose;
    private AcdTransitionType _acdTransitionType;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AnimationProperty)
        {
            Unload();

            var animation = change.GetNewValue<AcdAnimation?>();
            if (animation is { })
            {
                Load(animation);
            } 
        }
    }

    private void Load(AcdAnimation animation)
    {
        _frames = new List<BitmapFrame>();
        _acdTransitionType = animation.AcdTransitionType;

        for (var i = 0; i < animation.Frames.Count; i++)
        {
            var frame = animation.Frames[i];
            if (frame.Images.Count < 1)
            {
                // TODO;
                continue;
            }

            var image = frame.Images[0];
            if (image.Filename is null)
            {
                continue;
            }

            var bitmap = ImageLoader.ToBitmap(AcdLoader.BasePath, image.Filename);
            if (bitmap is { })
            {
                var duration = TimeSpan.FromMilliseconds(frame.Duration * 10);
                var bitmapFrame = new BitmapFrame(bitmap, duration);
                _frames.Add(bitmapFrame);
            }
        }

        // TODO: Frames are in reverse order in project.
        _frames.Reverse();

        if (_frames is { } && _frames.Count >= 0)
        {
            _lastFrameIndex = 0;
            _lastFrame = _frames[_lastFrameIndex];
            RenderFrame(_lastFrame);
        }
    }

    private void Unload()
    {
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

                if (_bitmapControl is { })
                {
                    _bitmapControl.Bitmap = bitmapFrame.Bitmap;
                }

                // TODO: Handle transition type, branching and exit branch.
                // TODO: AcdFrame.Branching.
                // TODO: AcdFrame.ExitBranch.
                // TODO: AcdFrame.SoundEffect.
                // TODO: AcdAnimation.AcdTransitionType.
                // var branching = frame.Branching;
                // var exitBranch = frame.ExitBranch;

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
