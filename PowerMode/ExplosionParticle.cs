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

        private readonly IAdornmentLayer adornmentLayer;
        /// <summary>
        /// Adornment image
        /// </summary>
        private Image image;


        private System.Windows.Rect _rect = new System.Windows.Rect(-5, -5, 5, 5);
        private EllipseGeometry geometry;
        [ThreadStatic]
        private static Random _random;

        private double _alphaRemoveAmount = 0.008;

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
            geometry = new EllipseGeometry(_rect);
        }
        public async Task Explode()
        {
            var upVelocity = (double)Random.NextDouble()*10;
            var leftVelocity = (double)Random.NextDouble() * 10*(Random.Next(0,2)==1?1:-1);
            var brush = new SolidColorBrush(_color);
            brush.Freeze();
            var drawing = new GeometryDrawing(brush, null, geometry);
            drawing.Freeze();

            var drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();
            this.image = new Image
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
                Canvas.SetLeft(this.image, _left);
                Canvas.SetTop(this.image, _top);

                // Add the image to the adornment layer and make it relative to the viewport
                this.adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, this.image, null);
                await Task.Delay(17);
                adornmentLayer.RemoveAdornment(image);
            }
        }
        
    }
}
