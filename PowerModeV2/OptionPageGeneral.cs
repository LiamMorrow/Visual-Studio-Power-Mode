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
        /// <summary>
        /// use this to force reloading of settings
        /// each time a setting changes here we increment the number
        /// each explosion particle checks it's version of options against
        /// this one to check for updates
        /// </summary>
        public static uint OptionsVersion = 0;

        [Category("Power Mode")]
        [DisplayName("Alpha Decrement Amount")]
        [Description("The amount of alpha removed every frame.")]
        public double AlphaRemoveAmount
        {
            get { return ExplosionParticle.AlphaRemoveAmount; }
            set
            {
                ExplosionParticle.AlphaRemoveAmount = value;
                OptionsVersion++;
            }
        }

        [Category("Power Mode")]
        [DefaultValue(true)]
        [DisplayName("Explosion Particle - get color from environment")]
        [Description(
            "Whether to get the color from the environment theme or not - overrides Explosion Particle Color value if set"
            )]
        public bool GetColorFromEnvironment
        {
            get { return ExplosionParticle.GetColorFromEnvironment; }
            set
            {
                ExplosionParticle.GetColorFromEnvironment = value;
                OptionsVersion++;
            }
        }

        [Category("Power Mode")]
        [DisplayName("Explosion Particle Color")]
        [Description("The color of the explosion particle")]
        public Color Color
        {
            get { return ExplosionParticle.Color; }
            set
            {
                ExplosionParticle.Color = value;
                OptionsVersion++;
            }
        }

        [Category("Power Mode")]
        [DisplayName("Explosion Particle Randomized Color")]
        [Description("Whether to use a random color. Overrides Explosion Particle Color if set.")]
        public bool RandomColor
        {
            get { return ExplosionParticle.RandomColor; }
            set
            {
                ExplosionParticle.RandomColor = value;
                OptionsVersion++;
            }
        }

        [Category("Power Mode")]
        [Description("Delay between Frames (milliseconds)")]
        [DisplayName("Frame Delay")]
        public int FrameDelay
        {
            get { return ExplosionParticle.FrameDelay; }
            set
            {
                ExplosionParticle.FrameDelay = value;
                OptionsVersion++;
            }
        }
        [Category("Power Mode")]
        [DisplayName("Particle Party Change Threshold")]
        [Description("The amount of change required to trigger a particle party")]
        [DefaultValue(20)]
        public int ParticlePartyChangeThreshold {
            get
            {
                return ExplosionViewportAdornment.ParticlePartyChangeThreshold;
            }
            set
            {
                ExplosionViewportAdornment.ParticlePartyChangeThreshold = value;
                OptionsVersion++;
            }
        }

        [Category("Power Mode")]
        [DisplayName("Particle Party Enabled")]
        [Description("Enable to create particles everywhere when a large change is made")]
        [DefaultValue(true)]
        public bool ParticlePartyEnabled {
            get
            {
                return ExplosionViewportAdornment.ParticlePartyEnabled;
            }
            set
            {
                ExplosionViewportAdornment.ParticlePartyEnabled = value;
                OptionsVersion++;
            }
        }

        [Category("Power Mode")]
        [DisplayName("Gravity")]
        [Description("The strength of the gravity")]
        public double Gravity
        {
            get { return ExplosionParticle.Gravity; }
            set
            {
                ExplosionParticle.Gravity = value;
                OptionsVersion++;
            }
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
        [DisplayName("Particles per keystroke")]
        [Description("The number of particles to show each key press")]
        public int ParticlePerPress
        {
            get { return ExplosionViewportAdornment.ParticlePerPress; }
            set { ExplosionViewportAdornment.ParticlePerPress = value; }
        }

        [Category("Power Mode")]
        [DisplayName("Max Side Velocity")]
        [Description("The maximum sideward velocity of the particles")]
        public double MaxSideVelocity
        {
            get { return ExplosionParticle.MaxSideVelocity; }
            set
            {
                ExplosionParticle.MaxSideVelocity = value;
                OptionsVersion++;
            }
        }

        [Category("Power Mode")]
        [DisplayName("Max Upwards Velocity")]
        [Description("The maximum upward velocity of the particles")]
        public double MaxUpVelocity
        {
            get { return ExplosionParticle.MaxUpVelocity; }
            set
            {
                ExplosionParticle.MaxUpVelocity = value;
                OptionsVersion++;
            }
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
            set
            {
                ExplosionParticle.StartAlpha = value;
                OptionsVersion++;
            }
        }

        [Category("Power Mode")]
        [DisplayName("Combo Threshold")]
        [Description("The number of keypresses required to turn on Power Mode. Set to 0 to always enable Power Mode.")]
        public int ComboThreshold
        {
            get { return ExplosionViewportAdornment.ComboActivationThreshold; }
            set { ExplosionViewportAdornment.ComboActivationThreshold = value; }
        }
    }
}