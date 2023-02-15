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
    public static readonly StyledProperty<DefineAnimation?> AnimationProperty = 
        AvaloniaProperty.Register<Player, DefineAnimation?>(nameof(Animation));

    public DefineAnimation? Animation
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
    private TransitionType _transitionType;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AnimationProperty)
        {
            Unload();

            var animation = change.GetNewValue<DefineAnimation?>();
            if (animation is { })
            {
                Load(animation);
            } 
        }
    }

    private void Load(DefineAnimation animation)
    {
        _frames = new List<BitmapFrame>();
        _transitionType = animation.TransitionType;

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
                // TODO: DefineFrame.Branching.
                // TODO: DefineFrame.ExitBranch.
                // TODO: DefineFrame.SoundEffect.
                // TODO: DefineAnimation.TransitionType.
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
