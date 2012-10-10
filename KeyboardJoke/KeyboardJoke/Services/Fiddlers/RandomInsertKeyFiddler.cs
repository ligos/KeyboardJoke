using System;
using Microsoft.SPOT;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services.Fiddlers
{
    public class RandomInsertKeyFiddler : IFiddler
    {
        private bool _IsComplete = false;
        private Random _Random;

        public void Initialise()
        {
            // No-op.
        }

        public void OnPublish(Random randomGenerator)
        {
            _IsComplete = false;
            _Random = randomGenerator;
        }

        public void ApplyOnKeyDown(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed, bool logoPressed)
        {
            // No-op.
        }

        public void ApplyOnKeyUp(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed, bool logoPressed)
        {
            // If this keypress corresponds to a typable character and no modifier keys are pressed: insert a random one.
            if (!altPressed && !ctlPressed && !logoPressed && KeyboardTables.KeyToChar(thisKeyPress, isShifted) != '\0')
            {
                var charTable = isShifted ? KeyboardTables.ShiftedKeyToCharTable : KeyboardTables.KeyToCharTable;
                var idx = _Random.Next(charTable.Length - 1);
                char theChosenKey = charTable[idx];
                if (theChosenKey == '\0')
                    // Didn't randomly choose a valid key: wait for next press.
                    return;

                // Delete previous key and echo the randomly chosen one.
                var keyStroke = KeyboardTables.CharToKeyStroke(theChosenKey);
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
