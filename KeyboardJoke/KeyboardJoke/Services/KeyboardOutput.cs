using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.USBClient;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class KeyboardOutput
    {
        private USBC_Keyboard _KeyboardClient;

        public void Start()
        {
            _KeyboardClient = USBClientController.StandardDevices.StartKeyboard();
        }

        public void Stop()
        {
            if (USBClientController.GetState() != USBClientController.State.Stopped)
                USBClientController.Stop();
            _KeyboardClient = null;
        }

        public void KeyDown(USBC_Key key)
        {
            if (USBClientController.GetState() != USBClientController.State.Running || _KeyboardClient == null)
                return;
            _KeyboardClient.KeyDown(key);
        }
        public void KeyUp(USBC_Key key)
        {
            if (USBClientController.GetState() != USBClientController.State.Running || _KeyboardClient == null)
                return;
            _KeyboardClient.KeyUp(key);
        }
    }
}
