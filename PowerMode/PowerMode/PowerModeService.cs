using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMode
{
    class PowerModeService : SPowerMode, IPowerMode
    {
        private readonly PowerModePackage _package;

        public PowerModeService(PowerModePackage package)
        {
            _package = package;
        }
        
        public PowerModePackage Package {
            get { return _package; }
        }
    }
}
