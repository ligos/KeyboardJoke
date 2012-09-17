using System;
using Microsoft.SPOT;

namespace MurrayGrant.KeyboardJoke.Entities
{
    public struct TinyTimeSpan
    {
        public static readonly TinyTimeSpan Zero = new TinyTimeSpan(0);
        private short _Milliseconds;

        public TinyTimeSpan(short milliseconds)
        {
            _Milliseconds = milliseconds;
        }
        public TinyTimeSpan(TimeSpan ts)
        {
            _Milliseconds = (short)(ts.Ticks / TimeSpan.TicksPerMillisecond);
        }

        public int Milliseconds { get { return _Milliseconds; } }
        public int Seconds { get { return _Milliseconds * 1000; } }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() == typeof(TinyTimeSpan))
                return Equals((TinyTimeSpan)obj);
            else if (obj.GetType() == typeof(TimeSpan))
                return Equals((TimeSpan)obj);
            else
                return false;

        }
        public bool Equals(TinyTimeSpan sts)
        {
            return this._Milliseconds == sts._Milliseconds;
        }
        public bool Equals(TimeSpan ts)
        {
            var ms = ts.Ticks / TimeSpan.TicksPerMillisecond;
            if (ms > Int16.MaxValue || ms < Int16.MinValue)
                return false;
            return this._Milliseconds == (short)ms;
        }
        public static bool operator ==(TinyTimeSpan sts1, TinyTimeSpan sts2)
        {
            return sts1.Equals(sts2);
        }
        public static bool operator !=(TinyTimeSpan sts1, TinyTimeSpan sts2)
        {
            return !sts1.Equals(sts2);
        }

        public override int GetHashCode()
        {
            return 31 ^ typeof(TinyTimeSpan).GetHashCode() ^ _Milliseconds.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(true);
        }
        public string ToString(bool pretty)
        {
            if (pretty)
                return _Milliseconds.ToString("N0") + "ms";
            else
                return _Milliseconds.ToString();
        }

        public TinyTimeSpan Add(short milliseconds)
        {
            return new TinyTimeSpan((short)(_Milliseconds + milliseconds));
        }
        public TinyTimeSpan Add(TinyTimeSpan sts)
        {
            return new TinyTimeSpan((short)(_Milliseconds + sts._Milliseconds));
        }
    }
}
