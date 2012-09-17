using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;

namespace MurrayGrant.KeyboardJoke.Services
{
    public delegate void RtcUpdateDelegate(DateTime newDateTime);

    public static class WallClock
    {
        // Make sure this is always set to UTC.
        // We can worry about conversions on the PC.

        public const UInt32 RtcValidMagic = 0x6d605ea1;        // This is set in BatteryRam when the RTC is valid.

        static WallClock()
        {
            // Establish if the RTC is valid or not based on a magic value set in battery RAM.
            var buf = new byte[4];
            BatteryRAM.Read(0, buf, 0, buf.Length);
            var batteryRamClockVal = Utility.ExtractValueFromArray(buf, 0, 4);
            RtcIsValid = batteryRamClockVal == RtcValidMagic;

            if (RtcIsValid)
                Utility.SetLocalTime(RealTimeClock.GetTime());
            else
                Utility.SetLocalTime(new DateTime(2011, 01, 01, 0, 0, 0));      // Fake the start of 2011.
        }

        /// <summary>
        /// True if the RTC has been set and battery has remained on.
        /// </summary>
        public static bool RtcIsValid { get; private set; }

        public static DateTime GetDateTime()
        {
            return DateTime.Now;
        }
        public static void SetDateTime(DateTime newTime)
        {
            var buf = new byte[4];
            Utility.InsertValueIntoArray(buf, 0, 4, RtcValidMagic);
            RealTimeClock.SetTime(newTime);     // Update RTC.
            BatteryRAM.Write(0, buf, 0, 4);     // Mark it valid in BatteryRam.
            RtcIsValid = true;                  // Mark it valid in code.
            Utility.SetLocalTime(newTime);      // Don't forget to update the framework clock too!

            var evt = OnRtcUpdate;
            if (evt != null)
                evt(newTime);
        }

        public static event RtcUpdateDelegate OnRtcUpdate;
    }
}
