using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;

namespace MurrayGrant.KeyboardJoke
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                Debug.Print("Constructing World()");
#if DEBUG
                Debug.EnableGCMessages(true);
#endif
                var cfg = new Configuration();
                cfg.LedPin = FEZ_Pin.Digital.LED;
                cfg.HasLcd = true;

                cfg.FiddleConfig = new FiddleConfig();
                cfg.FiddleConfig.DebugScaleingFactor = 0.07;
                cfg.FiddleConfig.InactivityTimeout = new TimeSpan(TimeSpan.TicksPerMinute * 10);
                cfg.FiddleConfig.MinTimeToFirstFiddle = new TimeSpan(TimeSpan.TicksPerMinute * 2);
                cfg.FiddleConfig.MinKeystrokesToFirstFiddle = 80;
                cfg.FiddleConfig.Definitions = new FiddleDefinition[1];

                int i = 0;
                //var phrases = new string[] { "Phrase", "Another", "Project", "Refactor" };
                //cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                //cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.InsertPhraseFiddler(phrases);
                //cfg.FiddleConfig.Definitions[i].Probability = Int32.MaxValue;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                //cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 2);
                //cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 5);
                //i++;

                //cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                //cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.DuplicateKeyFiddler();
                //cfg.FiddleConfig.Definitions[i].Probability = Int32.MaxValue;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                //cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 1);
                //cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 2);
                //i++;

                //cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                //cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.DeleteKeyFiddler();
                //cfg.FiddleConfig.Definitions[i].Probability = Int32.MaxValue;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                //cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 1);
                //cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 2);
                //i++;

                cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.DelayKeysFidder();
                cfg.FiddleConfig.Definitions[i].Probability = Int32.MaxValue;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 1);
                cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 2);
                i++;

                var world = new World();
                world.Run(cfg);
#if DEBUG
                Debug.Print("Entry thread sleeping forever");
#endif
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.Print("FATAL EXCEPTION");
                Debug.Print(ex.ToString());
#else
                Services.ExceptionService.Singleton.HandleException(ex);
#endif
            }
        }

    }
}
