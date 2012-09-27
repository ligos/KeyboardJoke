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
            // Choose a phrase.
            _SelectedPhrase = randomGenerator.Next(_Phrases.Length - 1);
            _IsComplete = false;
        }

        public void ApplyOnKeyDown(DelayBuffer output, byte thisKeyPress, bool isShifted)
        {
            // No-op.                                        
        }

        public void ApplyOnKeyUp(DelayBuffer output, byte thisKeyPress, bool isShifted)
        {
            // Ensure the phrase is inserted at the end of a sentence or paragraph.
            bool atEndOfSentence = (thisKeyPress == (byte)GHIElectronics.NETMF.USBClient.USBC_Key.Space) 
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

                // Mark completion.
                _IsComplete = true;
            }
            else
            {
                // Capture the last keypress as a character.
                var c = KeyboardTables.KeyToChar((byte)thisKeyPress, isShifted);
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
