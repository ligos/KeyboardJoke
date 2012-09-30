using System;
using Microsoft.SPOT;

namespace StructTests
{
    public class World
    {
        private byte[] _Bytes;
        private int[] _Ints;
        private short[] _Shorts;
        private long[] _Longs;
        private Double[] _Doubles;
        private TimeSpan[] _TimeSpans;
        private TinyTimeSpan[] _TinyTimeSpans;
        private JustAnInt32[] _WrappedInt32;
        private Guid[] _Guids;
        private QueuedEvent[] _QueuedEntities;
        private QueuedEventSimple[] _SimpleEvents;
        private QueuedEventSimpleROStatic[] _SimpleEventsEmptyStatic;
        private QueuedEventTimeSpan[] _EventsTimespan;
        private DifferentEntity[] _DifferentEntities;
        private MultipleEntities[] _MultipleEntities;
        private TwoDoublesAndTimeSpans[] _DoublesAndTimes;
        private DateTime[] _DateTimes;
        private Object[] _Objects;
        private UInt32[] _UInt32s;
        private Empty[] _EmptyStructs;

        public void SetArray(uint type, int size)
        {
            if (type == (1 << 0))
                _Bytes = new byte[size];
            else if (type == (1 << 1))
                _Ints = new int[size];
            else if (type == (1 << 2))
                _Shorts = new short[size];
            else if (type == (1 << 3))
                _Longs = new long[size];
            else if (type == (1 << 4))
                _Doubles = new Double[size];
            else if (type == (1 << 5))
                _TimeSpans = new TimeSpan[size];
            else if (type == (1 << 6))
                _TinyTimeSpans = new TinyTimeSpan[size];
            else if (type == (1 << 7))
                _WrappedInt32 = new JustAnInt32[size];
            else if (type == (1 << 8))
                _Guids = new Guid[size];
            else if (type == (1 << 9))
                _QueuedEntities = new QueuedEvent[size];
            else if (type == (1 << 10))
                _SimpleEvents = new QueuedEventSimple[size];
            else if (type == (1 << 11))
                _SimpleEventsEmptyStatic = new QueuedEventSimpleROStatic[size];
            else if (type == (1 << 12))
                _EventsTimespan = new QueuedEventTimeSpan[size];
            else if (type == (1 << 13))
                _DifferentEntities = new DifferentEntity[size];
            else if (type == (1 << 14))
                _MultipleEntities = new MultipleEntities[size];
            else if (type == (1 << 15))
                _DoublesAndTimes = new TwoDoublesAndTimeSpans[size];
            else if (type == (1 << 16))
                _DateTimes = new DateTime[size];
            else if (type == (1 << 17))
                _Objects = new Object[size];
            else if (type == (1 << 18))
                _UInt32s = new UInt32[size];
            else if (type == (1 << 19))
                _EmptyStructs = new Empty[size];
        }
        
        public string GetTypeName(uint type)
        {
            if (type == (1 << 0))
                return "byte";
            else if (type == (1 << 1))
                return "int";
            else if (type == (1 << 2))
                return "short";
            else if (type == (1 << 3))
                return "long";
            else if (type == (1 << 4))
                return "double";
            else if (type == (1 << 5))
                return "TimeSpan";
            else if (type == (1 << 6))
                return "TinyTimeSpan";
            else if (type == (1 << 7))
                return "JustAnInt32";
            else if (type == (1 << 8))
                return "Guid";
            else if (type == (1 << 9))
                return "QueuedEvent";
            else if (type == (1 << 10))
                return "QueuedEventSimple";
            else if (type == (1 << 11))
                return "QueuedEventSimpleROStatic";
            else if (type == (1 << 12))
                return "QueuedEventTimeSpan";
            else if (type == (1 << 13))
                return "DifferentEntity";
            else if (type == (1 << 14))
                return "MultipleEntities";
            else if (type == (1 << 15))
                return "TwoDoublesAndTimeSpans";
            else if (type == (1 << 16))
                return "DateTime";
            else if (type == (1 << 17))
                return "Object";
            else if (type == (1 << 18))
                return "UInt32";
            else if (type == (1 << 19))
                return "Empty";
            else
                return "<NONE>";
        }

        public void ClearAll()
        {
            _Bytes = new byte[0];
            _Ints = new int[0];
            _Shorts = new short[0];
            _Longs = new long[0];
            _Doubles = new Double[0];
            _TimeSpans = new TimeSpan[0];
            _TinyTimeSpans = new TinyTimeSpan[0];
            _WrappedInt32 = new JustAnInt32[0];
            _Guids = new Guid[0];
            _QueuedEntities = new QueuedEvent[0];
            _SimpleEvents = new QueuedEventSimple[0];
            _SimpleEventsEmptyStatic = new QueuedEventSimpleROStatic[0];
            _EventsTimespan = new QueuedEventTimeSpan[0];
            _DifferentEntities = new DifferentEntity[0];
            _MultipleEntities = new MultipleEntities[0];
            _DoublesAndTimes = new TwoDoublesAndTimeSpans[0];
            _DateTimes = new DateTime[0];
            _Objects = new Object[0];
            _UInt32s = new UInt32[0];
            _EmptyStructs = new Empty[0];
        }

        public uint GetMemoryUsage()
        {
            Debug.GC(true);
            return Debug.GC(true);
        }
    }
}
