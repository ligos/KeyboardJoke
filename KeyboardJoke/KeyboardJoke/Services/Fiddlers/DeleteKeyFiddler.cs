using System;
using Microsoft.SPOT;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services.Fiddlers
{
    public class DeleteKeyFiddler : IFiddler
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

        public void ApplyOnKeyDown(DelayBuffer output, byte thisKeyPress, bool isShifted)
        {
            // No-op.
        }

        public void ApplyOnKeyUp(DelayBuffer output, byte thisKeyPress, bool isShifted)
        {
            // If this keypress corresponds to a typable character, queue a backspace to delete it.
            var c = KeyboardTables.KeyToChar(thisKeyPress, isShifted);
            if (c != '\0')
            {
                output.KeyPress((byte)GHIElectronics.NETMF.USBClient.USBC_Key.BackSpace);
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
