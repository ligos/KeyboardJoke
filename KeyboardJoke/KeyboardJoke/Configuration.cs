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
        public TimeSpan InactivityTimeout { get; set; }         // After not seeing any keystrokes for this long, fiddles will not be selected.
        public TimeSpan MinTimeToFirstFiddle { get; set; }      // This amount of time AND below keystrokes must pass without inactivty to enable fiddles.
        public int MinKeystrokesToFirstFiddle { get; set; }     // This number of keystrokes AND above time must pass without inactivty to enable fiddles.
        public uint RandomSeed { get; set; }

        public FiddleDefinition[] Definitions { get; set; }
    }

    public class FiddleDefinition
    {
        public Services.Fiddlers.IFiddler Implementation { get; set; }

        public int Probability { get; set; }
        public TimeSpan MinDelay { get; set; }
        public TimeSpan MaxDelay { get; set; }

        public override string ToString()
        {
            return Implementation.GetType().Name;
        }
    }
}
