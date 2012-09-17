using System;
using Microsoft.SPOT;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class WorldUpdateRate
    {
        private TimeSpan _LastHeartbeat;    // 8 bytes
        private int _Iterations = 0;        // 4 bytes
        public int IterationsLastSecond { get; private set; }       // 4 bytes
        // 4 bytes for object reference
        // Total: 20 bytes.

        public void Init(TimeSpan heartBeat)
        {
            this._LastHeartbeat = heartBeat;
        }
        public void Update(TimeSpan heartBeat)
        {
            if (heartBeat.Seconds == this._LastHeartbeat.Seconds)
                this._Iterations++;
            else
            {
                this.IterationsLastSecond = this._Iterations;
                this._Iterations = 0;
                this._LastHeartbeat = heartBeat;
            }
        }
    }
}
