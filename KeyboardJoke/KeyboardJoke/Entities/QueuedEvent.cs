using System;
using System.Runtime.InteropServices;
using Microsoft.SPOT;

namespace MurrayGrant.KeyboardJoke.Entities
{
    // Because the Micro Framework uses so much overhead for value types,
    // we encode the QueuedEvents in a UInt32 and use extension methods to get at fields.

    // xx xx xx xx
    // High    Low
    // - Low byte = EventType
    // - High 3 bytes = arbitrary data (dependent on event type).
    // - Key Press = highest byte
    // - Delay in Milliseconds = middle two bytes.
    public static class QueuedEvent
    {
        public static UInt32 CreateDelay(short milliseconds)
        {
            return (((UInt32)milliseconds) << 16) | (UInt32)EventType.Delay;
        }
        public static UInt32 CreateKeyDown(KeyboardKey key)
        {
            return (((UInt32)key) << 24) | (UInt32)EventType.KeyDown;
        }
        public static UInt32 CreateKeyUp(KeyboardKey key)
        {
            return (((UInt32)key) << 24) | (UInt32)EventType.KeyUp;
        }
        public static UInt32 CreateKeyPress(KeyboardKey key)
        {
            return (((UInt32)key) << 24) | (UInt32)EventType.KeyPress;
        }

        public static EventType GetEventType(this UInt32 d)
        {
            return (EventType)(byte)d;
        }

        public static KeyboardKey GetKeyPressed(this UInt32 d)
        {
            return (KeyboardKey)(d >> 24);
        }
        public static GHIElectronics.NETMF.USBClient.USBC_Key GetKeyPressedAsClient(this UInt32 d)
        {
            return (GHIElectronics.NETMF.USBClient.USBC_Key)(d >> 24);
        }

        public static short GetDelay(this UInt32 d)
        {
            return (short)(d >> 16);
        }
    }

    public enum EventType : byte
    {
        None = 0,
        Delay = 1,
        KeyDown = 2,
        KeyUp = 3,
        KeyPress = 4,
    }
}
