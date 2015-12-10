/*
The MIT License(MIT)

Copyright(c) 2015 Liam Morrow

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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Media;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.Win32;

namespace PowerMode
{
    public class OptionPageGeneral : DialogPage
    {
        [Category("Power Mode")]
        [DisplayName("Alpha Decrement Amount")]
        [Description("The amount of alpha removed every frame.")]
        public double AlphaRemoveAmount
        {
            get { return ExplosionParticle.AlphaRemoveAmount; }
            set { ExplosionParticle.AlphaRemoveAmount = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Explosion Particle - get color from environment")]
        [Description("Whether to get the color from the environment theme or not - overrides Explosion Particle Color value if set")]
        public bool bGetColorFromEnvironment
        {
            get { return ExplosionParticle.bGetColorFromEnvironment; }
            set { ExplosionParticle.bGetColorFromEnvironment = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Explosion Particle Color")]
        [Description("The color of the explosion particle")]
        public Color Color
        {
            get { return ExplosionParticle.Color; }
            set { ExplosionParticle.Color = value; }
        }
        [Category("Power Mode")]
        [DisplayName("Explosion Particle Randomized Color")]
        [Description("Whether to use a random color. Overrides Explosion Particle Color if set.")]
        public bool RandomColor
        {
            get { return ExplosionParticle.RandomColor; }
            set { ExplosionParticle.RandomColor = value; }
        }

        [Category("Power Mode")]
        [Description("Delay between Frames (milliseconds)")]
        [DisplayName("Frame Delay")]
        public int FrameDelay
        {
            get { return ExplosionParticle.FrameDelay; }
            set { ExplosionParticle.FrameDelay = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Gravity")]
        [Description("The strength of the gravity")]

        public double Gravity
        {
            get { return ExplosionParticle.Gravity; }
            set { ExplosionParticle.Gravity = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Max Particle Count")]
        [Description("The maximum amount of particles at one time")]
        public int MaxParticleCount
        {
            get { return ExplosionParticle.MaxParticleCount; }
            set { ExplosionParticle.MaxParticleCount = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Max Side Velocity")]
        [Description("The maximum sideward velocity of the particles")]
        public double MaxSideVelocity
        {
            get { return ExplosionParticle.MaxSideVelocity; }
            set { ExplosionParticle.MaxSideVelocity = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Max Upwards Velocity")]
        [Description("The maximum upward velocity of the particles")]
        public double MaxUpVelocity
        {
            get { return ExplosionParticle.MaxUpVelocity; }
            set { ExplosionParticle.MaxUpVelocity = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Particles Enabled")]
        [Description("Sets whether the particles are enabled")]
        public bool ParticlesEnabled
        {
            get { return ExplosionViewportAdornment.ParticlesEnabled; }
            set { ExplosionViewportAdornment.ParticlesEnabled = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Screen Shake")]
        [Description("Sets whether the screen shakes")]
        public bool ShakeEnabled
        {
            get { return ExplosionViewportAdornment.ShakeEnabled; }
            set { ExplosionViewportAdornment.ShakeEnabled = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Start Alpha")]
        [Description("The starting opacity of the particle. Affects lifetime.")]
        public double StartAlpha
        {
            get { return ExplosionParticle.StartAlpha; }
            set { ExplosionParticle.StartAlpha = value; }
        }
    }

}
