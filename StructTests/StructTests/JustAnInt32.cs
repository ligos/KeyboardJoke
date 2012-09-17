using System;
using Microsoft.SPOT;

namespace StructTests
{
    public struct JustAnInt32
    {
        public readonly Int32 Value;

        public JustAnInt32(int value)
        {
            Value = value;
        }
    }
}
