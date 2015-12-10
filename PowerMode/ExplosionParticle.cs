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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using EnvDTE;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell;
using PowerMode.Extensions;

namespace PowerMode
{
    public class ExplosionParticle
    {
        [ThreadStatic]
        private static Random _random;

        private readonly DTE _service;
        private readonly IAdornmentLayer adornmentLayer;
        private double _left, _top;

        private PowerModePackage _optionsPackage;
        private Rect _rect = new Rect(-5, -5, 5, 5);

        private EllipseGeometry geometry;

        public static double AlphaRemoveAmount { get; set; } = 0.045;

        public static bool bGetColorFromEnvironment { get; set; } = false;

        public static Color Color { get; set; } = Colors.Black;
        public static bool RandomColor { get; set; } = false;

        public static int FrameDelay { get; set; } = 17;

        public static double Gravity { get; set; } = 0.3;

        public static int MaxParticleCount { get; set; } = int.MaxValue;

        public static double MaxSideVelocity { get; set; } = 2;

        public static double MaxUpVelocity { get; set; } = 10;

        public static double StartAlpha { get; set; } = 0.9;

        private static int ParticleCount { get; set; }

        private static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random();
                }
                return _random;
            }
        }

        public ExplosionParticle(IAdornmentLayer adornment, DTE service, double top, double left)
        {
            _left = left;
            adornmentLayer = adornment;
            _service = service;
            _top = top;
            geometry = new EllipseGeometry(_rect);
        }

        public async System.Threading.Tasks.Task Explode()
        {
            if (ParticleCount > MaxParticleCount)
                return;
            ParticleCount++;

            // TODO: rewrite this part for better design & performance
            // store service & package as static member.

            var service = ServiceProvider.GlobalProvider.GetService(typeof(SPowerMode));
            var pm_service = service as IPowerMode;
            var package = pm_service.Package;
            var page = package.General;

            ExplosionParticle.Color = page.Color;
            ExplosionParticle.AlphaRemoveAmount = page.AlphaRemoveAmount;
            ExplosionParticle.bGetColorFromEnvironment = bGetColorFromEnvironment;
            ExplosionParticle.RandomColor = page.RandomColor;
            ExplosionParticle.FrameDelay = page.FrameDelay;
            ExplosionParticle.Gravity = page.Gravity;
            ExplosionParticle.MaxParticleCount = page.MaxParticleCount;
            ExplosionParticle.MaxSideVelocity = page.MaxSideVelocity;
            ExplosionParticle.MaxUpVelocity = page.MaxUpVelocity;
            //ExplosionParticle.ParticlesEnabled = page.ParticlesEnabled;
            //ExplosionParticle.ShakeEnabled = page.ShakeEnabled;
            ExplosionParticle.StartAlpha = page.StartAlpha;

            // End of TODO.

            var alpha = StartAlpha;
            var upVelocity = Random.NextDouble() * MaxUpVelocity;
            var leftVelocity = Random.NextDouble() * MaxSideVelocity * Random.NextSignSwap();
            SolidColorBrush brush = null;
         
            if (bGetColorFromEnvironment)
            {
                var svc = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SVsUIShell)) as Microsoft.VisualStudio.Shell.Interop.IVsUIShell5;
                brush = new SolidColorBrush(Microsoft.VisualStudio.Shell.VsColors.GetThemedWPFColor(svc, Microsoft.VisualStudio.PlatformUI.EnvironmentColors.PanelTextColorKey));
            }
            else if (RandomColor)
            {
                brush = new SolidColorBrush(Random.NextColor());
            }
            else
            {
                brush = new SolidColorBrush(Color);
            }
            brush.Freeze();
            var drawing = new GeometryDrawing(brush, null, geometry);
            drawing.Freeze();

            var drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();
            var image = new Image
            {
                Source = drawingImage,
            };
            while (alpha >= AlphaRemoveAmount)
            {
                _left -= leftVelocity;
                _top -= upVelocity;
                upVelocity -= Gravity;
                alpha -= AlphaRemoveAmount;

                image.Opacity = alpha;

                Canvas.SetLeft(image, _left);
                Canvas.SetTop(image, _top);
                try
                {
                    // Add the image to the adornment layer and make it relative to the viewport
                    adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative,
                        null,
                        null,
                        image,
                        null);
                    await System.Threading.Tasks.Task.Delay(FrameDelay);
                    adornmentLayer.RemoveAdornment(image);
                }
                catch
                {
                    break;
                }
            }
            try
            {
                adornmentLayer.RemoveAdornment(image);
            }
            catch
            {
                //Ignore all errors, not critical
            }
            ParticleCount--;
        }
    }
}