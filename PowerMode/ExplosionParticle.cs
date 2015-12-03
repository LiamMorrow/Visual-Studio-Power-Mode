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
using Microsoft.VisualStudio.Text.Editor;

namespace PowerMode
{
    public class ExplosionParticle
    {
        private static double _alphaRemoveAmount = 0.045;
        private static Color _color = Colors.Black;

        private static int _frameDelay = 17;

        private static double _gravity = 0.3;

        private static int _maxParticleCount = int.MaxValue;

        private static int _particleCount;

        [ThreadStatic]
        private static Random _random;

        private static double _startAlpha = 0.9;
        private readonly IAdornmentLayer adornmentLayer;
        private double _left, _top;

        private System.Windows.Rect _rect = new System.Windows.Rect(-5, -5, 5, 5);

        private EllipseGeometry geometry;

        public double AlphaRemoveAmount
        {
            get { return _alphaRemoveAmount; }
            set { _alphaRemoveAmount = value; }
        }

        public Color Color { get { return _color; } set { _color = value; } }

        public int FrameDelay
        {
            get { return _frameDelay; }
            set { _frameDelay = value; }
        }

        public double Gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }

        public int MaxParticleCount
        {
            get { return _maxParticleCount; }
            set { _maxParticleCount = value; }
        }

        public double StartAlpha
        {
            get { return _startAlpha; }
            set { _startAlpha = value; }
        }

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

        private int ParticleCount
        {
            get { return _particleCount; }
            set { _particleCount = value; }
        }

        public ExplosionParticle()
        {
        }

        public ExplosionParticle(IAdornmentLayer adornment, double top, double left)
        {
            _left = left;
            adornmentLayer = adornment;
            _top = top;
            geometry = new EllipseGeometry(_rect);
        }

        public async Task Explode()
        {
            if (ParticleCount > MaxParticleCount)
                return;
            ParticleCount++;
            var alpha = StartAlpha;
            var upVelocity = Random.NextDouble() * 10;
            var leftVelocity = Random.NextDouble() * 2 * (Random.Next(0, 2) == 1 ? 1 : -1);
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
            while (alpha >= _alphaRemoveAmount)
            {
                _left -= leftVelocity;
                _top -= upVelocity;
                upVelocity -= Gravity;
                alpha -= _alphaRemoveAmount;

                image.Opacity = alpha;

                Canvas.SetLeft(image, _left);
                Canvas.SetTop(image, _top);
                try
                {
                    // Add the image to the adornment layer and make it relative to the viewport
                    this.adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative,
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
            }
            ParticleCount--;
        }
    }
}