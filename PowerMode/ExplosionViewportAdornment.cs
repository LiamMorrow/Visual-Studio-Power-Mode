﻿/*
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

        private bool isShaking;

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
            if (e.Changes != null && (e.EditTag == null || e.EditTag.GetType().Name == "TextEditAction"))
            {
                for (int i = 0; i < e.Changes.Count; i++)
                {
                    try
                    {
                        HandleChange();
                    }
                    catch
                    {
                        //Ignore, not critical that we catch it
                    }
                }
                if (ShakeEnabled && !isShaking) // Prevent and additional shaking when we are already shaking
                {
                    isShaking = true;
                    await Shake(e.Changes.Sum(x=>x.Delta)); // do all shaking at once
                    isShaking = false;
                }
            }
        }

        private void HandleChange()
        {
            if (ParticlesEnabled)
            {
                for (var i = 0; i < 10; i++)
                {
                    var explosion = new ExplosionParticle(_adornmentLayer,
                        (DTE)Package.GetGlobalService(typeof(DTE)),
                        _view.Caret.Top,
                        _view.Caret.Left);
                    explosion.Explode();
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

        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            FormatCode(e);
        }

        private void TextBuffer_PostChanged(object sender, EventArgs e)
        {
        }
    }
}