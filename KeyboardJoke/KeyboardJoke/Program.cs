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
