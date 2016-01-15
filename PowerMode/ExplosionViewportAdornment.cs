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
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using PowerMode.Extensions;
using Package = Microsoft.VisualStudio.Shell.Package;

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
        /// The layer for the adornment.
        /// </summary>
        private readonly IAdornmentLayer _adornmentLayer;

        /// <summary>
        /// Text view to add the adornment on.
        /// </summary>
        private readonly IWpfTextView _view;

        public static bool ParticlesEnabled { get; set; } = true;

        public static bool ShakeEnabled { get; set; } = true;

        public int ExplosionAmount { get; set; } = 2;

        public int ExplosionDelay { get; set; } = 50;

        public int MaxShakeAmount { get; set; } = 5;

        /// <summary>
        /// Store the last time the user pressed a key. 
        /// To maintain a combo, the user must keep pressing keys at least every x seconds.
        /// </summary>
        public DateTime LastKeyPress { get; set; } = DateTime.Now;

        public int ComboStreak { get; set; }

        public static int ComboActivationThreshold { get; set; } = 0;

        public static int ComboTimeout { get; set; } = 10000; // In milliseconds

        private const int MinMsBetweenShakes = 10;

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
            _adornmentLayer = view.GetAdornmentLayer("ExplosionViewportAdornment");
        }

        private async void FormatCode(TextContentChangedEventArgs e)
        {
            if ((DateTime.Now - LastKeyPress).TotalMilliseconds < MinMsBetweenShakes)
            {
                return;
            }

            if (e.Changes != null && e.Changes.Count > 0)
            {
                try
                {
                    await HandleChange(e.Changes.Sum(x => x.Delta));
                }
                catch
                {
                    //Ignore, not critical that we catch it
                }
            }
        }

        private async Task HandleChange(int delta)
        {
            if (ComboCheck())
            {
                if (ParticlesEnabled)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var explosion = new ExplosionParticle(_adornmentLayer,
                            (DTE)Package.GetGlobalService(typeof(DTE)),
                            _view.Caret.Top,
                            _view.Caret.Left);
                        var expl = explosion.Explode();

#pragma warning disable CS4014 // Don't care about return
                        Task.Run(() => expl);
#pragma warning restore CS4014
                    }
                }
                if (ShakeEnabled)
                {
                    await Shake(delta);
                }
            }
        }

        private async Task Shake(int delta)
        {
            for (int i = 0; i < Math.Abs(delta) && i < MaxShakeAmount; i++)
            {
                int leftAmount = ExplosionAmount * Random.NextSignSwap(),
                    topAmount = ExplosionAmount * Random.NextSignSwap();

                _view.ViewportLeft += leftAmount;
                _view.ViewScroller.ScrollViewportVerticallyByPixels(topAmount);
                await Task.Delay(ExplosionDelay);
                _view.ViewportLeft -= leftAmount;
                _view.ViewScroller.ScrollViewportVerticallyByPixels(-topAmount);
            }
        }

        /// <summary>
        /// Keep track of how many keypresses the user has done and returns whether power mode should be activated for each change.
        /// </summary>
        /// <returns>True if power mode should be activated for this change. False if power mode should be ignored for this change.</returns>
        private bool ComboCheck()
        {
            var now = DateTime.Now;
            ComboStreak++;

            if (LastKeyPress.AddMilliseconds(ComboTimeout) < now) // More than x ms since last key-press. Combo has been broken.
            {
                ComboStreak = 1;
            }

            LastKeyPress = now;
            var activatePowerMode = ComboStreak >= ComboActivationThreshold; // Activate powermode if number of keypresses exceeds the threshold for activation
            return activatePowerMode; // Perhaps different levels for power-mode intensity? First just particles, then screen shake?
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