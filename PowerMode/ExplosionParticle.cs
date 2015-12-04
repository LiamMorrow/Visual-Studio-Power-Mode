/*
   Copyright 2015 Liam Morrow

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
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

        private PowerModeOptionsPackage _optionsPackage;
        private Rect _rect = new Rect(-5, -5, 5, 5);

        private EllipseGeometry geometry;

        public double AlphaRemoveAmount
        {
            get
            {
                var dte = _service;
                var props = dte.Properties["Power Mode", "General"];
                return (double)props.Item(nameof(OptionPageGrid.AlphaRemoveAmount)).Value;
            }
        }

        public Color Color
        {
            get
            {
                var dte = _service;
                var props = dte.Properties["Power Mode", "General"];
                return (Color)props.Item(nameof(OptionPageGrid.Color)).Value;
            }
        }

        public int FrameDelay
        {
            get
            {
                var dte = _service;
                var props = dte.Properties["Power Mode", "General"];
                return (int)props.Item(nameof(OptionPageGrid.FrameDelay)).Value;
            }
        }

        public double Gravity
        {
            get
            {
                var dte = _service;
                var props = dte.Properties["Power Mode", "General"];
                return (double)props.Item(nameof(OptionPageGrid.Gravity)).Value;
            }
        }

        public int MaxParticleCount
        {
            get
            {
                var dte = _service;
                var props = dte.Properties["Power Mode", "General"];
                return (int)props.Item(nameof(OptionPageGrid.MaxParticleCount)).Value;
            }
        }

        public double MaxSideVelocity
        {
            get
            {
                var dte = _service;
                var props = dte.Properties["Power Mode", "General"];
                return (double)props.Item(nameof(OptionPageGrid.MaxSideVelocity)).Value;
            }
        }

        public double MaxUpVelocity
        {
            get
            {
                var dte = _service;
                var props = dte.Properties["Power Mode", "General"];
                return (double)props.Item(nameof(OptionPageGrid.MaxUpVelocity)).Value;
            }
        }

        public double StartAlpha
        {
            get
            {
                var dte = _service;
                var props = dte.Properties["Power Mode", "General"];
                return (double)props.Item(nameof(OptionPageGrid.StartAlpha)).Value;
            }
        }

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

        public async Task Explode()
        {
            if (ParticleCount > MaxParticleCount)
                return;
            ParticleCount++;
            var alpha = StartAlpha;
            var upVelocity = Random.NextDouble() * MaxUpVelocity;
            var leftVelocity = Random.NextDouble() * MaxSideVelocity * Random.NextSignSwap();
            var brush = new SolidColorBrush(Color);
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
                    await Task.Delay(FrameDelay);
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