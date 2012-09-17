using System;
using System.Runtime.InteropServices;
using Microsoft.SPOT;

namespace StructTests
{
    //[StructLayout(LayoutKind.Sequential)]
    public struct QueuedEvent
    {
        //public readonly static QueuedEvent Empty = new QueuedEvent();

        public readonly EventType Type;
        public readonly byte KeyPressed;
        public readonly TinyTimeSpan Delay;


        public QueuedEvent(TinyTimeSpan delay)
        {
            Type = EventType.Delay;
            Delay = delay;
            KeyPressed = 0;
        }
        public QueuedEvent(EventType type, byte key)
        {
            if (!(type == EventType.KeyDown || type == EventType.KeyUp || type == EventType.KeyPress))
                throw new ArgumentException("Invalid event type: " + type.ToString(), "type");
            Type = type;
            KeyPressed = key;
            Delay = TinyTimeSpan.Zero;
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


    public struct QueuedEventSimple
    {
        public readonly byte Type;
        public readonly byte KeyPressed;
        public readonly short DelayMilliseconds;
    }
    public struct QueuedEventSimpleROStatic
    {
        public static readonly QueuedEventSimpleROStatic Empty = new QueuedEventSimpleROStatic();

        public readonly byte Type;
        public readonly byte KeyPressed;
        public readonly short DelayMilliseconds;
    }

    public struct QueuedEventTimeSpan
    {
        public readonly byte Type;
        public readonly byte KeyPressed;
        public readonly TimeSpan Delay;
    }
}
