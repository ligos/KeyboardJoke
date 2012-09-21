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
        private KeyboardInput _KeyboardInput;
        private KeyboardOutput _KeyboardOutput;
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
                _KeyboardOutput = new KeyboardOutput();
                if (!_DebuggerOnUsb)
                    _KeyboardOutput.Start();

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
                _KeyboardInput = new KeyboardInput(_UI, _KeyboardOutput, _FiddleConfig, _DebuggerOnUsb);
                _KeyboardInput.BeginMonitorInputFrom(device);
            }
        }
        #endregion
    }
}

