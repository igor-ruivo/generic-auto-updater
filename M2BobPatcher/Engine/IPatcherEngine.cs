using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.Engine {
    interface IPatcherEngine {
        bool hasMetadataFile();
        void generateMetadataFile();
        void downloadServerMetadataFile();
        string[] compareMetadata();
        void download();
    }
}
