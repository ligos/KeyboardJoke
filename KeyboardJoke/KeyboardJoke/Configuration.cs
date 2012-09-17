using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.FEZ;

namespace MurrayGrant.KeyboardJoke
{
    public class Configuration
    {
        public FEZ_Pin.Digital LedPin { get; set; }
        public bool ShowWorldUpdateRate { get; set; }
        public bool HasLcd { get; set; }
    }
}
