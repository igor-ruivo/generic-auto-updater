using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M2BobPatcher.Engine {
    interface IPatcherEngine {
        void Patch();
        void Repair();
    }
}
