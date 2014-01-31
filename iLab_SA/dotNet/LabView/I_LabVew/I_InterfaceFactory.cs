using System;
using System.Collections.Generic;
using System.Text;

namespace iLabs.LabView
{
    public interface I_InterfaceFactory
    {
        I_LabViewInterface GetLabViewInterface();

        I_LabViewInterface GetLabViewInterface(string revision);
    }
}
