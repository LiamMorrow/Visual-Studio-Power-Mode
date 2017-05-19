/*
The MIT License (MIT)

Copyright (c) 2015 Liam Morrow

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using EnvDTE;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using PowerMode.Extensions;

namespace PowerMode
{
    public class ExplosionParticle
    {
        [ThreadStatic]
        private static Random _random;

        private static Rect _rect = new Rect(-5, -5, 5, 5);

        private static readonly EllipseGeometry geometry = new EllipseGeometry(_rect);

        private readonly Action<ExplosionParticle> _afterExplode;

        private readonly DTE _service;

        private readonly IAdornmentLayer adornmentLayer;

        private Image _image;

        private double _iterations;

        private DoubleAnimation _leftAnimation;

        private DoubleAnimation _opacityAnimation;

        private uint _optionsVersion = 0;

        private DoubleAnimation _topAnimation;

        static ExplosionParticle()
        {
            var service = ServiceProvider.GlobalProvider.GetService(typeof(IPowerModeService)) as IPowerMode;
            if (service == null)
            {
                return;
            }

            var page = service.Package.General;

            Color = page.Color;
            AlphaRemoveAmount = page.AlphaRemoveAmount;
            GetColorFromEnvironment = GetColorFromEnvironment;
            RandomColor = page.RandomColor;
            FrameDelay = page.FrameDelay;
            Gravity = page.Gravity;
            MaxParticleCount = page.MaxParticleCount;
            MaxSideVelocity = page.MaxSideVelocity;
            MaxUpVelocity = page.MaxUpVelocity;
            StartAlpha = page.StartAlpha;
        }

        public ExplosionParticle(IAdornmentLayer adornment, DTE service, Action<ExplosionParticle> afterExplode)
        {
            adornmentLayer = adornment;
            _service = service;
            _afterExplode = afterExplode;
            InitializeOptions();
        }

        public static double AlphaRemoveAmount { get; set; } = 0.045;

        public static Color Color { get; set; } = Colors.Black;
        public static int FrameDelay { get; set; } = 17;
        public static bool GetColorFromEnvironment { get; set; }
        public static double Gravity { get; set; } = 0.3;
        public static int MaxParticleCount { get; set; } = int.MaxValue;
        public static double MaxSideVelocity { get; set; } = 2;
        public static double MaxUpVelocity { get; set; } = 10;
        public static bool RandomColor { get; set; }
        public static double StartAlpha { get; set; } = 0.9;

        private static int ParticleCount { get; set; }

        private static Random Random => _random ?? (_random = new Random());

        public void Explode(double top, double left)
        {
            if (ParticleCount > MaxParticleCount)
            {
                return;
            }

            ParticleCount++;
            if (_optionsVersion != OptionPageGeneral.OptionsVersion)
            {
                InitializeOptions();
            }

            var upVelocity = Random.NextDouble() * MaxUpVelocity;
            var leftVelocity = Random.NextDouble() * MaxSideVelocity * Random.NextSignSwap();
            _leftAnimation.From = left;
            _leftAnimation.To = left - (_iterations * leftVelocity);
            _topAnimation.From = top;
            _topAnimation.By = -upVelocity;
            _image.Visibility = Visibility.Visible;
            _image.BeginAnimation(Canvas.LeftProperty, _leftAnimation);
            _image.BeginAnimation(Canvas.TopProperty, _topAnimation);
            _image.BeginAnimation(UIElement.OpacityProperty, _opacityAnimation);

            ParticleCount--;
        }

        private void InitializeOptions()
        {
            Color brushColor;
            if (GetColorFromEnvironment)
            {
                var svc = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell5;
                brushColor = svc.GetThemedWPFColor(EnvironmentColors.PanelTextColorKey);
            }
            else if (RandomColor)
            {
                brushColor = Random.NextColor();
            }
            else
            {
                brushColor = Color;
            }
            var brush = new SolidColorBrush(brushColor);
            var drawing = new GeometryDrawing(brush, null, geometry);
            drawing.Freeze();

            var drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();
            _image = new Image
            {
                Source = drawingImage,
                Visibility = Visibility.Hidden
            };
            // Add the image to the adornment layer and make it relative to the viewport
            adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative,
                null,
                null,
                _image,
                null);

            _iterations = StartAlpha / AlphaRemoveAmount;
            var timeSpan = TimeSpan.FromMilliseconds(FrameDelay * _iterations);

            _leftAnimation = new DoubleAnimation()
            {
                Duration = timeSpan
            };
            _topAnimation = new DoubleAnimation()
            {
                EasingFunction = new BackEase { Amplitude = Gravity * 35 },
                Duration = timeSpan
            };
            _opacityAnimation = new DoubleAnimation()
            {
                From = StartAlpha,
                To = StartAlpha - (_iterations * AlphaRemoveAmount),
                Duration = timeSpan
            };
            _opacityAnimation.Completed += (sender, args) => OnAnimationComplete();
            _optionsVersion = OptionPageGeneral.OptionsVersion;
        }

        private void OnAnimationComplete()
        {
            _image.Visibility = Visibility.Hidden;
            _afterExplode?.Invoke(this);
        }
    }
}