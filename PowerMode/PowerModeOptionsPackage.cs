//------------------------------------------------------------------------------
// <copyright file="PowerModeOptionsPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

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
    public class OptionPageGrid : DialogPage
    {
        [Category("Power Mode")]
        [DisplayName("Alpha Decrement Amount")]
        [Description("The amount of alpha removed every frame.")]
        public static double AlphaRemoveAmount { get; set; } = 0.045;

        [Category("Power Mode")]
        [DisplayName("Explosion Particle Color")]
        [Description("The color of the explosion particle")]
        public static Color Color { get; set; } = Colors.Black;

        [Category("Power Mode")]
        [Description("Delay between Frames (milliseconds)")]
        [DisplayName("Frame Delay")]
        public static int FrameDelay { get; set; } = 17;

        [Category("Power Mode")]
        [DisplayName("Gravity")]
        [Description("The strength of the gravity")]
        public static double Gravity { get; set; } = 0.3;

        [Category("Power Mode")]
        [DisplayName("Max Particle Count")]
        [Description("The maximum amount of particles at one time")]
        public static int MaxParticleCount { get; set; } = int.MaxValue;

        [Category("Power Mode")]
        [DisplayName("Start Alpha")]
        [Description("The starting opacity of the particle. Affects lifetime.")]
        public static double StartAlpha { get; set; } = 0.9;
    }

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#1110", "#1112", "1.0", IconResourceID = 1400)] // Info on this package for Help/About
    [Guid(PowerModeOptionsPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(OptionPageGrid),
    "Power Mode", "General", 1114, 1113, true)]
    [ProvideProfile(typeof(OptionPageGrid),
    "Power Mode", "General", 1114, 1113, isToolsOptionPage: true, DescriptionResourceID = 1115)]
    public sealed class PowerModeOptionsPackage : Package
    {
        /// <summary>
        /// PowerModeOptionsPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "4e687eae-ae26-4139-b888-a0ae8c2e16ff";

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerModeOptionsPackage"/> class.
        /// </summary>
        public PowerModeOptionsPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        #endregion Package Members
    }
}