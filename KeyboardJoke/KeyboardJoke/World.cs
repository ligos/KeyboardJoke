using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using MurrayGrant.KeyboardJoke.Services;
using GHIElectronics.NETMF.USBHost;

namespace MurrayGrant.KeyboardJoke
{
    public class World
    {
        private LedBlinker _HeartBeat;
        private UserInterface _UI;
        private KeyboardAndMouseInput _KeyboardAndMouseInput;
        private KeyboardAndMouseOutput _KeyboardAndMouseOutput;
        private FiddleConfig _FiddleConfig;
        private bool _DebuggerOnUsb;

        #region Init
        public void Run(Configuration cfg)
        {
            var initStart = Utility.GetMachineTime();
            Debug.Print("Starting Init(): " + initStart);

            // Determine if the USB debugger is available.
            var debugInterface = GHIElectronics.NETMF.Hardware.Configuration.DebugInterface.GetCurrent();
            _DebuggerOnUsb = (debugInterface == GHIElectronics.NETMF.Hardware.Configuration.DebugInterface.Port.USB1);

            if (_DebuggerOnUsb)
                Debug.EnableGCMessages(true);

            // Lcd / UI.
            LcdAndKeypad lcd = null;
            if (cfg.HasLcd)
            {
                lcd = new LcdAndKeypad();
                lcd.Init();
            }

            // Pass the file writer and lcd to the exception handler.
            ExceptionService.Singleton.Init(lcd);

            try
            {
                // More high level UI.
                this._UI = new UserInterface(lcd, _DebuggerOnUsb);
                this._UI.Start();
                if (lcd != null) lcd.SetCursorPosition(0, 3);

                // Heatbeat LED.
                this._HeartBeat = new LedBlinker(cfg.LedPin);
                if (_DebuggerOnUsb)
                    this._HeartBeat.Start();

                // Start up the keyboard client.
                _KeyboardAndMouseOutput = new KeyboardAndMouseOutput();
                if (!_DebuggerOnUsb)
                    _KeyboardAndMouseOutput.Start();

                // Monitor USB host devices being added to detect a keyboard.
                _FiddleConfig = cfg.FiddleConfig;
                USBHostController.DeviceConnectedEvent += new USBH_DeviceConnectionEventHandler(USBHostController_DeviceConnectedEvent);

                var initEnd = Utility.GetMachineTime();
                Debug.Print("Completed Init(): " + initEnd);
                Debug.Print("Total Init(): " + initEnd.Subtract(initStart));
            }
            catch (Exception ex)
            {
                ExceptionService.Singleton.HandleException(ex);
            }
        }

        void USBHostController_DeviceConnectedEvent(USBH_Device device)
        {
            if (device.TYPE == USBH_DeviceType.Keyboard)
            {
                if (_KeyboardAndMouseInput == null)
                    _KeyboardAndMouseInput = new KeyboardAndMouseInput(_UI, _KeyboardAndMouseOutput, _FiddleConfig, _DebuggerOnUsb);
                _KeyboardAndMouseInput.BeginMonitorKeyboardFrom(device);
            }
            else if (device.TYPE == USBH_DeviceType.Mouse)
            {
                if (_KeyboardAndMouseInput == null)
                    _KeyboardAndMouseInput = new KeyboardAndMouseInput(_UI, _KeyboardAndMouseOutput, _FiddleConfig, _DebuggerOnUsb);
                _KeyboardAndMouseInput.BeginMonitorMouseFrom(device);
            }
        }
        #endregion
    }
}

