using System;
using Microsoft.SPOT;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services.Fiddlers
{
    public class DuplicateKeyFiddler : IFiddler
    {
        private bool _IsComplete = false;

        public void Initialise()
        {
            // No-op.
        }

        public void OnPublish(Random randomGenerator)
        {
            _IsComplete = false;
        }

        public void ApplyOnKeyDown(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted)
        {
            // No-op.
        }

        public void ApplyOnKeyUp(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted)
        {
            // If this keypress corresponds to a typable character, duplicate it.
            var c = KeyboardTables.KeyToChar(thisKeyPress, isShifted);
            if (c != '\0')
            {
                var keyStroke = KeyboardTables.CharToKeyStroke(c);
                output.KeyPressWithModifier(keyStroke);
                _IsComplete = true;
            }
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
