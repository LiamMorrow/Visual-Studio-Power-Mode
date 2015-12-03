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
        [ThreadStatic]
        private static Random _random;

        /// <summary>
        /// Text view to add the adornment on.
        /// </summary>
        private readonly IWpfTextView _view;

        /// <summary>
        /// The layer for the adornment.
        /// </summary>
        private readonly IAdornmentLayer adornmentLayer;

        private int _explosionAmount = 2;
        private int _explosionDelay = 50;
        private int _maxShakeAmount = 5;

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

        private async void FormatCode(TextContentChangedEventArgs e)
        {
            if (e.Changes != null)
            {
                for (int i = 0; i < e.Changes.Count; i++)
                {
                    try
                    {
                        await HandleChange(e.Changes[i].Delta);
                    }
                    catch
                    {
                        //Ignore, not critical that we catch it
                    }
                }
            }
        }

        private async Task HandleChange(int delta)
        {
            var tags = _view.Caret.ContainingTextViewLine.GetAdornmentTags("");
            for (var i = 0; i < 10; i++)
            {
                var explosion = new ExplosionParticle(adornmentLayer, _view.Caret.Top, _view.Caret.Left);
                var expl = explosion.Explode();
                Task.Run(() => expl);
            }
            await Shake(delta);
        }

        private async Task Shake(int delta)
        {
            for (int i = 0; i < Math.Abs(delta) && i < _maxShakeAmount; i++)
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
        }

        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            FormatCode(e);
        }

        private void TextBuffer_PostChanged(object sender, EventArgs e)
        {
        }
    }
}