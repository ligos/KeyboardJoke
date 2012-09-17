using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.USBHost;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class KeyboardInput
    {
        private readonly UserInterface _UI;
        private USBH_Keyboard _Keyboard;
        private readonly DelayBuffer _OutBuffer;

        public KeyboardInput(UserInterface ui, KeyboardOutput output)
        {
            _UI = ui;
            _OutBuffer = new DelayBuffer(output, ui);
        }

        public void BeginMonitorInputFrom(USBH_Device device)
        {
            _Keyboard = new USBH_Keyboard(device);
            _Keyboard.KeyUp += new USBH_KeyboardEventHandler(_HostKeyboard_KeyUp);
            _Keyboard.KeyDown += new USBH_KeyboardEventHandler(_HostKeyboard_KeyDown);
            _Keyboard.Disconnected += new USBH_KeyboardEventHandler(_Keyboard_Disconnected);
        }


        void _Keyboard_Disconnected(USBH_Keyboard sender, USBH_KeyboardEventArgs args)
        {
            // Unhook event handlers and null our host keyboard.
            _Keyboard.KeyUp -= new USBH_KeyboardEventHandler(_HostKeyboard_KeyUp);
            _Keyboard.KeyDown -= new USBH_KeyboardEventHandler(_HostKeyboard_KeyDown);
            _Keyboard.Disconnected -= new USBH_KeyboardEventHandler(_Keyboard_Disconnected);
            _Keyboard = null;
        }

        void _HostKeyboard_KeyDown(USBH_Keyboard sender, USBH_KeyboardEventArgs args)
        {
            // Ways to fiddle input:
            //  - Delay key strokes by an increasing amount for a few seconds, so it appears the keyboard is lagging.
            //  - Duplicate key strokes at random.
            //  - Insert additional phrases

            // Must have an inital delay period before anything happens so logins can happen OK.
            //  - A certain number of keystrokes and time must pass.


            if (_UI.KeyEchoDelay.Milliseconds > 0)
                _OutBuffer.Delay((short)_UI.KeyEchoDelay.Milliseconds);
            _OutBuffer.KeyDown((GHIElectronics.NETMF.USBClient.USBC_Key)args.Key);
        }

        void _HostKeyboard_KeyUp(USBH_Keyboard sender, USBH_KeyboardEventArgs args)
        {
            _OutBuffer.KeyUp((GHIElectronics.NETMF.USBClient.USBC_Key)args.Key);
        }
    }
}
