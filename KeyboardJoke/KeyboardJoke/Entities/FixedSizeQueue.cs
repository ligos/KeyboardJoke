using System;
using System.Collections;
using Microsoft.SPOT;

namespace MurrayGrant.KeyboardJoke.Entities
{
    public class FixedSizeQueue
    {
        private readonly UInt32[] _Array;
        private int _Head = 0;
        private int _Tail = 0;

        public FixedSizeQueue(int size)
        {
            _Array = new UInt32[size];
        }


        public bool IsEmpty { get { return _Head == _Tail; } }
        public int Count { get { return _Head > _Tail ? _Head - _Tail : (_Array.Length - _Head) + _Tail; } }
        public int Capacity { get { return _Array.Length; } }
        public int SpaceRemaining { get { return Capacity - Count; } }

        public void Enqueue(UInt32 x)
        {
            lock (_Array)
            {
                _Array[_Head] = x;
                
                _Head++;
                if (_Head >= _Array.Length)
                    _Head = 0;

                if (_Head == _Tail)
                    throw new Exception("FixedSizedQueue exceeded capacity.");
            }
        }
        public UInt32 Dequeue()
        {
            UInt32 result;
            if (TryDequeue(out result))
                return result;
            else
                return 0;
        }
        public bool TryDequeue(out UInt32 result)
        {
            lock (_Array)
            {
                // Nothing in the queue.
                if (_Tail == _Head)
                {
                    result = 0;
                    return false;
                }

                result = _Array[_Tail];

                _Tail++;
                if (_Tail >= _Array.Length)
                    _Tail = 0;
                
                return true;
            }
        }
    }
}
