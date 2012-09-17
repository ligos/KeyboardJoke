using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class LedBlinker
    {
        private const int LedBlinkRateOnMilliseconds = 100;     // 4 bytes.
        private const int LedBlinkRateOffMilliseconds = 1400;   // 4 bytes.
        private TimeSpan LedBlinkRateOn { get { return new TimeSpan(LedBlinkRateOnMilliseconds * TimeSpan.TicksPerMillisecond); } }
        private TimeSpan LedBlinkRateOff { get { return new TimeSpan(LedBlinkRateOffMilliseconds * TimeSpan.TicksPerMillisecond); } }
        private bool _BlinkingLedState = true;      // 1 byte
        private OutputPort _LED;                    // 4 bytes
        private Timer _Timer;                       // 4 bytes

        // 4 bytes for object reference
        // total: 21 bytes

        public LedBlinker(FEZ_Pin.Digital pin)
        {
            this._LED = new OutputPort((Cpu.Pin)pin, false);
        }

        public void Start()
        {
            if (_Timer != null)
                _Timer.Dispose();
            // LED and jumper to select non-USB debugger are shared: if we aren't using USB debugger we can't use the LED.
            if (GHIElectronics.NETMF.Hardware.Configuration.DebugInterface.GetCurrent() != GHIElectronics.NETMF.Hardware.Configuration.DebugInterface.Port.USB1)
                return;

            this._Timer = new Timer(Blink, null, 0, LedBlinkRateOnMilliseconds);
        }
        public void Stop()
        {
            if (_Timer != null)
            {
                _Timer.Change(Timeout.Infinite, Timeout.Infinite);
                _Timer.Dispose();
            }
        }

        private void Blink(object obj)
        {
            if (ExceptionService.Singleton.HasExceptionHappened)
                return;
            if (this._Timer == null)
                return;

            try
            {
                // Update Led blink.
                this._LED.Write(this._BlinkingLedState);
                this._BlinkingLedState = !this._BlinkingLedState;

                // Re-schedule the timer for next transition.
                if (this._BlinkingLedState)
                    this._Timer.Change(LedBlinkRateOffMilliseconds, Timeout.Infinite);
                else
                    this._Timer.Change(LedBlinkRateOnMilliseconds, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                ExceptionService.Singleton.HandleException(ex);
            }
        }
    }
}
