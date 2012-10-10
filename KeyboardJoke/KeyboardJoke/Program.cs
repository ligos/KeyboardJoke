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
#else
                Debug.EnableGCMessages(false);
#endif
                var cfg = new Configuration();
                cfg.LedPin = FEZ_Pin.Digital.LED;
                cfg.HasLcd = true;
                
                cfg.FiddleConfig = new FiddleConfig();
                cfg.FiddleConfig.RandomSeed = 0xDC0786AF;
                cfg.FiddleConfig.DebugScaleingFactor = 0.07;
                cfg.FiddleConfig.InactivityTimeout = new TimeSpan(TimeSpan.TicksPerMinute * 5);
                cfg.FiddleConfig.MinTimeToFirstFiddle = new TimeSpan(TimeSpan.TicksPerMinute * 10);
                cfg.FiddleConfig.MinKeystrokesToFirstFiddle = 300;
                cfg.FiddleConfig.Definitions = new FiddleDefinition[6];

                int i = 0;
                var phrases = new string[] 
                { 
                    "DO MY DISHES.", 
                    "Please don't do that. Really, just don't.", 
                    "Brad, your keyboard hates you.", 
                    "Did you really say that??!?", 
                    "No Brad, I can't let you do that.",
                    "Must be infinite monkeys typing or something.",
                    "Round 1. Fight!",
                    "How much wood could a woodchuck chuck if a woodchuck could chuck wood?",
                    "I know what you did last summer",
                };
                cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.InsertPhraseFiddler(phrases);
                cfg.FiddleConfig.Definitions[i].Probability = 0x00000000;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 10);
                cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 20);
                i++;

                cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.DelayKeysFidder();
                cfg.FiddleConfig.Definitions[i].Probability = 0x00800000;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 10);
                cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 20);
                i++;

                cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.DeleteKeyFiddler();
                cfg.FiddleConfig.Definitions[i].Probability = 0x02000000;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 1);
                cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 3);
                i++;

                cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.TransposeKeysFiddler();
                cfg.FiddleConfig.Definitions[i].Probability = 0x06000000;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 1);
                cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 3);
                i++;

                cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.RandomInsertKeyFiddler();
                cfg.FiddleConfig.Definitions[i].Probability = 0x12000000;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 1);
                cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 3);
                i++;

                cfg.FiddleConfig.Definitions[i] = new FiddleDefinition();
                cfg.FiddleConfig.Definitions[i].Implementation = new Services.Fiddlers.DuplicateKeyFiddler();
                cfg.FiddleConfig.Definitions[i].Probability = 0x38000000;          // Probabilities should scale from 0 to Int32.MaxValue-1 and be sorted accordingly.
                cfg.FiddleConfig.Definitions[i].MinDelay = new TimeSpan(TimeSpan.TicksPerMinute * 1);
                cfg.FiddleConfig.Definitions[i].MaxDelay = new TimeSpan(TimeSpan.TicksPerMinute * 3);
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
