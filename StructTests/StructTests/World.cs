using System;
using Microsoft.SPOT;

namespace StructTests
{
    public class World
    {
        public readonly byte[] _Bytes;
        public readonly int[] _Ints;
        public readonly short[] _Shorts;
        public readonly long[] _Longs;
        public readonly Double[] _Doubles;
        public readonly TimeSpan[] _TimeSpans;
        public readonly TinyTimeSpan[] _TinyTimeSpans;
        public readonly JustAnInt32[] _WrappedInt32;
        public readonly Guid[] _Guids;
        public readonly QueuedEvent[] _QueuedEntities;
        public readonly QueuedEventSimple[] _SimpleEvents;
        public readonly QueuedEventSimpleROStatic[] _SimpleEventsEmptyStatic;
        public readonly QueuedEventTimeSpan[] _EventsTimespan;
        public readonly DifferentEntity[] _DifferentEntities;
        public readonly MultipleEntities[] _MultipleEntities;
        public readonly TwoDoublesAndTimeSpans[] _DoublesAndTimes;
        public readonly DateTime[] _DateTimes;
        public readonly Object[] _Objects;
        public readonly UInt32[] _UInt32s;

        public World(int size)
        {
            _UInt32s = new UInt32[size];
        }

        public int GetLength()
        {
            //return 0;
            return _UInt32s.Length;
        }
    }
}
