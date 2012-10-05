using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.USBClient;
using GHIElectronics.NETMF.USBHost;
using MurrayGrant.KeyboardJoke.Drivers;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class KeyboardAndMouseOutput
    {
        private USBC_KeyboardAndMouse _KeyboardMouseClient;

        public void Start()
        {
            _KeyboardMouseClient = USBC_KeyboardAndMouse.StartDefault();
        }

        public void Stop()
        {
            if (USBClientController.GetState() != USBClientController.State.Stopped)
                USBClientController.Stop();
            _KeyboardMouseClient = null;
        }

        public void KeyDown(USBC_Key key)
        {
            if (USBClientController.GetState() != USBClientController.State.Running || _KeyboardMouseClient == null)
                return;
            _KeyboardMouseClient.KeyDown(key);
        }
        public void KeyUp(USBC_Key key)
        {
            if (USBClientController.GetState() != USBClientController.State.Running || _KeyboardMouseClient == null)
                return;
            _KeyboardMouseClient.KeyUp(key);
        }

        public void MouseData(int dx, int dy, int dw, USBC_Mouse.Buttons buttons)
        {
            if (USBClientController.GetState() != USBClientController.State.Running || _KeyboardMouseClient == null)
                return;
            _KeyboardMouseClient.SendMouseData(dx, dy, dw, buttons);
        }
    }
}
