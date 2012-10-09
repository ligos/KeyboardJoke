using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services.Fiddlers
{
    public class DelayKeysFidder : IFiddler
    {
        private bool _IsComplete = false;
        // All these are in milliseconds.
        private short _CurrentDelay;
        private const short _InitialDelay = 100;
        private const short _DelayIncrement = 50;
        private const short _MaxDelay = 2000;
        private TimeSpan _KeyPressDeltaToAbort = new TimeSpan(TimeSpan.TicksPerMillisecond * 4000);
        private TimeSpan _LastDetectedKeyPress;

        public void Initialise()
        {
            // No-op.
        }

        public void OnPublish(Random randomGenerator)
        {
            _IsComplete = false;
            _CurrentDelay = _InitialDelay;
            _LastDetectedKeyPress = TimeSpan.MinValue;
        }

        public void ApplyOnKeyDown(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed, bool logoPressed)
        {
            // Record the initial keypress time after publish.
            if (_LastDetectedKeyPress < TimeSpan.Zero)
                _LastDetectedKeyPress = Utility.GetMachineTime();
        }

        public void ApplyOnKeyUp(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed, bool logoPressed)
        {
            var now = Utility.GetMachineTime();

            // Exit conditions: exceeded the maximum delay OR user has delayed too long between key presses (and has probably noticed the delay).
            if (_CurrentDelay > _MaxDelay || now.Subtract(_LastDetectedKeyPress).CompareTo(_KeyPressDeltaToAbort) > 0)
            {
                _IsComplete = true;
                return;
            }

            // If this keypress corresponds to a typable character, queue a delay after it.
            var c = KeyboardTables.KeyToChar(thisKeyPress, isShifted);
            if (c != '\0')
            {
                output.Delay(_CurrentDelay);

                // Increment the current delay for the next keypress.
                _CurrentDelay += _DelayIncrement;
            }
            
            // Keep the time of the last key press for next call.
            _LastDetectedKeyPress = now;
        }

        public bool IsComplete
        {
            get { return _IsComplete; }
        }

        public void AfterCompletion()
        {
            // No-op.
        }

    }
}
