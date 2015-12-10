
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace PowerMode
{

    [Guid("0F4D79E8-1BDA-4852-BAFF-39E8CC87E1D5")]
    [ComVisible(true)]
    public interface IPowerMode
    {
        PowerModePackage Package { get; }
    }

    [Guid("623E2A73-18DD-4855-ADF5-5400CA4C4EB5")]
    public interface SPowerMode { }
}
