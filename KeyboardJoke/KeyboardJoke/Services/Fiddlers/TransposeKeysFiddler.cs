using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services.Fiddlers
{
    public class TransposeKeysFiddler : IFiddler
    {
        private bool _IsComplete = false;
        private char _LastChar;

        public void Initialise()
        {
            // No-op.
        }

        public void OnPublish(Random randomGenerator)
        {
            _LastChar = '\0';
            _IsComplete = false;
        }

        public void ApplyOnKeyDown(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed, bool logoPressed)
        {
            // No-op.
        }

        public void ApplyOnKeyUp(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed, bool logoPressed)
        {
            var c = KeyboardTables.KeyToChar(thisKeyPress, isShifted);
            bool thisKeyValid = (c != '\0' && !altPressed && !ctlPressed && !logoPressed);
            if (!thisKeyValid)
            {
                // If this isn't valid, reset the previously memorised key.
                _LastChar = '\0';
                return;
            }

            // If this keypress corresponds to a typable character and no modifier keys are pressed and the last character is valid: transpose them.
            if (_LastChar != '\0')
            {
                // Delete the two characters.
                output.KeyPress(KeyboardKey.BackSpace);
                output.KeyPress(KeyboardKey.BackSpace);
                
                // Echo them in reverse order.
                output.KeyPressWithModifier(KeyboardTables.CharToKeyStroke(c));
                output.KeyPressWithModifier(KeyboardTables.CharToKeyStroke(_LastChar));

                // That's the end of this fiddle.
                _IsComplete = true;
                return;
            }

            // Memorise the key for next call.
            _LastChar = c;
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
