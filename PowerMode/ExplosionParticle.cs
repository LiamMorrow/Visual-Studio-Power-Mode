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
    class ExplosionParticle
    {
        private double _alpha = 0.5;
        private Color _color = Colors.BlueViolet;
        private double _left, _top;
        private IWpfTextView _view;
        private const double gravity = 0.3;
        public static int ParticleCount { get; set; }
        private readonly IAdornmentLayer adornmentLayer;
        public int MaxParticleCount { get; set; } = 50;

        private System.Windows.Rect _rect = new System.Windows.Rect(-5, -5, 5, 5);
        private EllipseGeometry geometry;
        [ThreadStatic]
        private static Random _random;

        private double _alphaRemoveAmount = 0.008;
        public static int FrameDelay { get; set; } = 17;
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
        public ExplosionParticle(IWpfTextView view,IAdornmentLayer adornment, double top, double left)
        {
            _left = left;
            adornmentLayer = adornment;
            _top = top;
            _view = view;
            _alpha = Random.NextDouble();
            geometry = new EllipseGeometry(_rect);
        }
        private async Task WaitForParticleSpace()
        {
            while (ParticleCount > MaxParticleCount)
            {
                await Task.Delay(FrameDelay);
            }
        }
        public async Task Explode()
        {
            await WaitForParticleSpace();
            ParticleCount++;
            var upVelocity = Random.NextDouble()*10;
            var leftVelocity = Random.NextDouble() * 5*(Random.Next(0,2)==1?1:-1);
            var brush = new SolidColorBrush(_color);
            brush.Freeze();
            var drawing = new GeometryDrawing(brush, null, geometry);
            drawing.Freeze();

            var drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();
            var image = new Image
            {
                Source = drawingImage,
            };
            while (_alpha >= _alphaRemoveAmount)
            {
                _left -= leftVelocity;
                _top -= upVelocity;
                upVelocity -= gravity;
                _alpha -= _alphaRemoveAmount;

                // Draw a square with the created brush and pen

                image.Opacity = _alpha;

                // Place the image in the top right hand corner of the Viewport
                Canvas.SetLeft(image, _left);
                Canvas.SetTop(image, _top);

                // Add the image to the adornment layer and make it relative to the viewport
                this.adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, image, null);
                await Task.Delay(FrameDelay);
                adornmentLayer.RemoveAdornment(image);
            }
            ParticleCount--;
        }
        
    }
}
