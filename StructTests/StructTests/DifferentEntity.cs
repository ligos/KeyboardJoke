using System;
using Microsoft.SPOT;

namespace StructTests
{
    public struct DifferentEntity
    {
        public readonly Double D;
        public readonly TimeSpan T;

        public DifferentEntity(double d, TimeSpan t)
        {
            D = d;
            T = t;
        }
    }

    public struct MultipleEntities
    {
        public readonly DifferentEntity E1;
        public readonly DifferentEntity E2;

        public MultipleEntities(DifferentEntity e1, DifferentEntity e2)
        {
            E1 = e1;
            E2 = e2;
        }
    }

    public struct TwoDoublesAndTimeSpans
    {
        public readonly double D1;
        public readonly TimeSpan T1;
        public readonly double D2;
        public readonly TimeSpan T2;
    }
}
