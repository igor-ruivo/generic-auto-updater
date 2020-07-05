using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.Engine {
    interface IPatcherEngine {
        void patch();
        void repair();
    }
}
