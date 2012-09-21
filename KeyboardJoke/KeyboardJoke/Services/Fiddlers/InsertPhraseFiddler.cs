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
        private Random _Rand;
        private bool _IsComplete;
        private byte _LastKeyPress;
        
        public InsertPhraseFiddler(string[] phrases)
        {
            _Phrases = phrases;
        }
        public void Initialise(Random randomGenerator)
        {
            _Rand = randomGenerator;
        }


        public void OnPublish()
        {
            // Choose a phrase.
            _SelectedPhrase = _Rand.Next(_Phrases.Length - 1);
            _IsComplete = false;
        }

        public void ApplyOnKeyDown(DelayBuffer output, byte thisKeyPress)
        {
            // No-op.                                        
        }

        public void ApplyOnKeyUp(DelayBuffer output, byte thisKeyPress)
        {
            // TODO: ensure the phrase is inserted at the end of a sentence or paragraph.
            bool atEndOfSentence = true;

            if (atEndOfSentence)
            {
                // Actually insert the phrase!!
                var keyStrokes = KeyboardTables.StringToKeyStrokes(_Phrases[_SelectedPhrase]);
                for (int i = 0; i < keyStrokes.Length; i++)
                    output.KeyPress((GHIElectronics.NETMF.USBClient.USBC_Key)keyStrokes[i]);

                // Mark completion.
                _IsComplete = true;
            }
            else
            {
                // Simply capture the last keypress.
                _LastKeyPress = thisKeyPress;    
            }

            
        }

        public bool IsComplete
        {
            get { return _IsComplete; }
        }

        public void AfterCompletion()
        {
            // Clear the last keypress.
            _LastKeyPress = 0;
        }
    }
}
