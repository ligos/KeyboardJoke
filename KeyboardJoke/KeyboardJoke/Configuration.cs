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

        public FiddleConfig FiddleConfig { get; set; }

    }

    public class FiddleConfig
    {
        public double DebugScaleingFactor { get; set; }

        public FiddleDefinition[] Definitions { get; set; }
    }

    public class FiddleDefinition
    {
        public Services.Fiddlers.IFiddler Implementation { get; set; }

        public int Probability { get; set; }
        public TimeSpan MinDelay { get; set; }
        public TimeSpan MaxDelay { get; set; }
    }
}
