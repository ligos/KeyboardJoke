using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.USBHost;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services.Fiddlers
{
    public class InsertPhraseFiddler : IFiddler
    {
        private readonly string[] _Phrases;
        private int _SelectedPhrase;
        private bool _IsComplete;
        private char _LastLetterPressed;
        
        public InsertPhraseFiddler(string[] phrases)
        {
            _Phrases = phrases;
        }
        public void Initialise()
        {
            // No-op.
        }


        public void OnPublish(Random randomGenerator)
        {
            // Because this fiddler is so blatent and obvious, only apply it if the unit has been running for at least 24 hours.
            if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks < TimeSpan.TicksPerHour * 24)
            {
                _IsComplete = true;
                return;
            }

            // Choose a phrase.
            _SelectedPhrase = randomGenerator.Next(_Phrases.Length - 1);
            _IsComplete = false;
        }

        public void ApplyOnKeyDown(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed, bool logoPressed)
        {
            // No-op.                                        
        }

        public void ApplyOnKeyUp(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed, bool logoPressed)
        {
            // Ensure the phrase is inserted at the end of a sentence or paragraph.
            bool atEndOfSentence = (thisKeyPress == KeyboardKey.Space) 
                && Array.IndexOf(KeyboardTables.EndOfSentenceCharacters, _LastLetterPressed) != -1;

            if (atEndOfSentence)
            {
                // Actually insert the phrase!!
                var s = _Phrases[_SelectedPhrase];
                for (int i = 0; i < s.Length; i++)
                {
                    var keyStroke = KeyboardTables.CharToKeyStroke(s[i]);
                    output.KeyPressWithModifier(keyStroke);
                }
                // Append a space.
                if (s[s.Length - 1] != ' ')
                    output.KeyPress(KeyboardKey.Space);

                // Mark completion.
                _IsComplete = true;
            }
            else
            {
                // Capture the last keypress as a character.
                var c = KeyboardTables.KeyToChar(thisKeyPress, isShifted);
                if (c != (char)0)
                    _LastLetterPressed = c;    
            }
        }

        public bool IsComplete
        {
            get { return _IsComplete; }
        }

        public void AfterCompletion()
        {
            // Clear the last letter pressed.
            _LastLetterPressed = (char)0;
        }
    }
}
