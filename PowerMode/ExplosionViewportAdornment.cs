//------------------------------------------------------------------------------
// <copyright file="ExplosionViewportAdornment.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace PowerMode
{
    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    internal sealed class ExplosionViewportAdornment
    {
        private int _explosionAmount = 2;
        private int _explosionDelay = 50;

        /// <summary>
        /// Text view to add the adornment on.
        /// </summary>
        private readonly IWpfTextView _view;

        /// <summary>
        /// The layer for the adornment.
        /// </summary>
        private readonly IAdornmentLayer adornmentLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosionViewportAdornment"/> class.
        /// Creates a square image and attaches an event handler to the layout changed event that
        /// adds the the square in the upper right-hand corner of the TextView via the adornment layer
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public ExplosionViewportAdornment(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            _view = view;
            _view.TextBuffer.Changed += TextBuffer_Changed;
            _view.TextBuffer.PostChanged += TextBuffer_PostChanged;


            this.adornmentLayer = view.GetAdornmentLayer("ExplosionViewportAdornment");

        }

        private void TextBuffer_PostChanged(object sender, EventArgs e)
        {
        }

        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            FormatCode(e);
        }

        private async void FormatCode(TextContentChangedEventArgs e)
        {
            if (e.Changes != null)
            {
                for (int i = 0; i < e.Changes.Count; i++)
                {
                    try
                    {
                        await HandleChange();
                    }
                    catch
                    {
                    }
                }
            }
        }

        [ThreadStatic] private static Random _random;

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

        private async Task Shake()
        {
            int leftAmount, topAmount;
            switch (Random.Next(0, 2))
            {
                case 0:
                    leftAmount = _explosionAmount;
                    break;
                case 1:
                    leftAmount = -_explosionAmount;
                    break;
                default:
                    leftAmount = 0;
                    break;
            }
            switch (Random.Next(0, 2))
            {
                case 0:
                    topAmount = _explosionAmount;
                    break;
                case 1:
                    topAmount = -_explosionAmount;
                    break;
                default:
                    topAmount = 0;
                    break;
            }
            _view.ViewportLeft += leftAmount;
            _view.ViewScroller.ScrollViewportVerticallyByPixels(topAmount);
            await Task.Delay(_explosionDelay);
            _view.ViewportLeft -= leftAmount;
            _view.ViewScroller.ScrollViewportVerticallyByPixels(-topAmount);
        }

        private async Task HandleChange()
        {
            for (var i = 0; i < 10; i++)
            {
                var explosion = new ExplosionParticle(this._view, adornmentLayer, _view.Caret.Top, _view.Caret.Left);
                var expl = explosion.Explode();
                Task.Run(() => expl);
            }
            Task.Run(() => Shake());
        }
    }
}
