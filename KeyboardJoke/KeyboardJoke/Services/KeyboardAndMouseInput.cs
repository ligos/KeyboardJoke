using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.USBHost;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class KeyboardAndMouseInput
    {
        // Mutable variables representing current state.
        private TimeSpan _MinimumDelay;                 // To delay based on startup / after period of inactivity.
        private int _MinimumKeystrokes;                 // To delay based on startup / after period of inactivity.
        private FiddleDefinition _SelectedFiddle;       // This is set with the timer when the next fiddle definition is selected.
        private FiddleDefinition _PublishedFiddle;      // When the timer elapses this is set, it is checked on each key press to action the fiddle.
        private readonly Timer _NextFiddleEvents;       // Once a fiddle is selected, this timer sets when it gets published and applied.
        private readonly Timer _InactivityTimer;        // To detect a period of inactivity.
        private bool _IsInactive;                       // True when inactive, false otherwise.
        private GHIElectronics.NETMF.USBClient.USBC_Mouse.Buttons _MouseButtonState;        // Bit field.
     
        // Immutable (or mostly immutable) state (eg: configuration).
        private USBH_Keyboard _Keyboard;
        private USBH_Mouse _Mouse;
        private KeyboardAndMouseOutput _Output;
        private readonly DelayBuffer _OutBuffer;
        private readonly bool _InDebugMode;
        private readonly UserInterface _Ui;
        private Random _Rand;
        private readonly FiddleConfig _Config;

        public bool ShiftPressed { get; private set; }

        public KeyboardAndMouseInput(UserInterface ui, KeyboardAndMouseOutput output, FiddleConfig config, bool inDebugMode)
        {
            _Ui = ui;
            _OutBuffer = new DelayBuffer(output, ui);
            _Output = output;
            _Config = config;
            _NextFiddleEvents = new Timer(this.FiddleHandler, null, Timeout.Infinite, Timeout.Infinite);
            _InactivityTimer = new Timer(this.InactivityTimerHandler, null, Timeout.Infinite, Timeout.Infinite);
            _InDebugMode = inDebugMode;
            _Rand = new Random((int)(Utility.GetMachineTime().Ticks & 0x00000000ffffffff));
            _MinimumDelay = TimeSpan.MaxValue;
            _MinimumKeystrokes = Int32.MaxValue;

        }

        public void BeginMonitorMouseFrom(USBH_Device device)
        {
            _Mouse = new USBH_Mouse(device);
            _Mouse.Disconnected +=new USBH_MouseEventHandler(Mouse_Disconnected);
            _Mouse.MouseDown +=new USBH_MouseEventHandler(Mouse_Activity);
            _Mouse.MouseUp += new USBH_MouseEventHandler(Mouse_Activity);
            _Mouse.MouseMove += new USBH_MouseEventHandler(Mouse_Activity);
            _Mouse.MouseWheel += new USBH_MouseEventHandler(Mouse_Activity);
        }

        private void Mouse_Activity(USBH_Mouse sender, USBH_MouseEventArgs args)
        {
            // Translate from the host button presses (normal enum) to client button presses (flags enum).
            GHIElectronics.NETMF.USBClient.USBC_Mouse.Buttons buttonBit = GHIElectronics.NETMF.USBClient.USBC_Mouse.Buttons.BUTTON_NONE;
            if (args.ChangedButton == USBH_MouseButton.Left)
                buttonBit = GHIElectronics.NETMF.USBClient.USBC_Mouse.Buttons.BUTTON_LEFT;
            else if (args.ChangedButton == USBH_MouseButton.Right)
                buttonBit = GHIElectronics.NETMF.USBClient.USBC_Mouse.Buttons.BUTTON_RIGHT;
            else if (args.ChangedButton == USBH_MouseButton.Middle)
                buttonBit = GHIElectronics.NETMF.USBClient.USBC_Mouse.Buttons.BUTTON_MIDDLE;
            else if (args.ChangedButton == USBH_MouseButton.XButton1)
                buttonBit = GHIElectronics.NETMF.USBClient.USBC_Mouse.Buttons.BUTTON_XBUTTON1;
            else if (args.ChangedButton == USBH_MouseButton.XButton2)
                buttonBit = GHIElectronics.NETMF.USBClient.USBC_Mouse.Buttons.BUTTON_XBUTTON2;
            if (args.ButtonState == USBH_MouseButtonState.Pressed)
                _MouseButtonState = _MouseButtonState | buttonBit;
            else
                _MouseButtonState = _MouseButtonState & ~buttonBit;

            _Output.MouseData(args.DeltaPosition.X, args.DeltaPosition.Y, args.DeltaPosition.ScrollWheelValue, _MouseButtonState);

            // Handle inactivity and minimum time before start state.
            this.SetInactivityTimeout();

            if (_IsInactive)
            {
                // Transition to initial delay state by initialising the minimum variables.
                SetMinimumCounters();
                _IsInactive = false;
            }
        }

        private void Mouse_Disconnected(USBH_Mouse sender, USBH_MouseEventArgs args)
        {
            // Unhook event handlers and null our host mouse device.
            _Mouse.Disconnected -= new USBH_MouseEventHandler(Mouse_Disconnected);
            _Mouse.MouseDown -= new USBH_MouseEventHandler(Mouse_Activity);
            _Mouse.MouseUp -= new USBH_MouseEventHandler(Mouse_Activity);
            _Mouse.MouseMove -= new USBH_MouseEventHandler(Mouse_Activity);
            _Mouse = null;
        }

        public void BeginMonitorKeyboardFrom(USBH_Device device)
        {
            _Keyboard = new USBH_Keyboard(device);
            _Keyboard.KeyUp += new USBH_KeyboardEventHandler(_HostKeyboard_KeyUp);
            _Keyboard.KeyDown += new USBH_KeyboardEventHandler(_HostKeyboard_KeyDown);
            _Keyboard.Disconnected += new USBH_KeyboardEventHandler(_Keyboard_Disconnected);

            // Create a random seed using:
            uint seed = _Config.RandomSeed;         // A random number from the internet!
            seed = seed ^ (uint)(((uint)device.VENDOR_ID << 16) | device.PRODUCT_ID);       // The USB vendor and device ids.
            seed = seed ^ (uint)(device.ID << 9);          // Some other USB ID.
            seed = seed ^ (uint)(Utility.GetMachineTime().Ticks & 0x00000000ffffffff);      // The current time.
            _Rand = new Random((int)seed);

            // Nothing is scheduled up front, we have to wait for the minimum time & keystrokes before anything happens.
            SetMinimumCounters();
            _NextFiddleEvents.Change(Timeout.Infinite, Timeout.Infinite);
            _InactivityTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _PublishedFiddle = null;
            _SelectedFiddle = null;
            _IsInactive = false;
        }


        private void _Keyboard_Disconnected(USBH_Keyboard sender, USBH_KeyboardEventArgs args)
        {
            // Unhook event handlers and null our host keyboard.
            _Keyboard.KeyUp -= new USBH_KeyboardEventHandler(_HostKeyboard_KeyUp);
            _Keyboard.KeyDown -= new USBH_KeyboardEventHandler(_HostKeyboard_KeyDown);
            _Keyboard.Disconnected -= new USBH_KeyboardEventHandler(_Keyboard_Disconnected);
            _Keyboard = null;

            // Null any events / state.
            _NextFiddleEvents.Change(Timeout.Infinite, Timeout.Infinite);
            _InactivityTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _PublishedFiddle = null;
            _SelectedFiddle = null;
            _MinimumKeystrokes = Int32.MaxValue;
            _MinimumDelay = TimeSpan.MaxValue;
            _IsInactive = false;
            _Ui.CurrentState = UnitState.None;
        }

        private void _HostKeyboard_KeyDown(USBH_Keyboard sender, USBH_KeyboardEventArgs args)
        {
            // Handle the key event ASAP.
            var key = (KeyboardKey)args.Key;
            _OutBuffer.KeyDown(key);
            this.ShiftPressed = (_Keyboard.GetKeyState(USBH_Key.LeftShift) == USBH_KeyState.Down) || (_Keyboard.GetKeyState(USBH_Key.RightShift) == USBH_KeyState.Down);
            
            // Handle inactivity and minimum time before start state.
            this.SetInactivityTimeout();

            if (_IsInactive)
            {
                // Transition to initial delay state by initialising the minimum variables.
                SetMinimumCounters();
                _IsInactive = false;
            }

            if (_PublishedFiddle != null)
            {
                // Call fiddler to adjust our output.
                _PublishedFiddle.Implementation.ApplyOnKeyDown(_OutBuffer, key, this.ShiftPressed);
                if (_PublishedFiddle.Implementation.IsComplete)
                {
                    // Fiddle was applied, schedule the next one.
                    _PublishedFiddle.Implementation.AfterCompletion();
                    _PublishedFiddle = null;
                    _Ui.FiddlesMade = _Ui.FiddlesMade + 1;
                    SelectNextFiddle();
                }
            }
        }

        private void _HostKeyboard_KeyUp(USBH_Keyboard sender, USBH_KeyboardEventArgs args)
        {
            // Handle the key press ASAP.
            var key = (KeyboardKey)args.Key;
            _OutBuffer.KeyUp(key);
            this.ShiftPressed = (_Keyboard.GetKeyState(USBH_Key.LeftShift) == USBH_KeyState.Down) || (_Keyboard.GetKeyState(USBH_Key.RightShift) == USBH_KeyState.Down);

            // Handle inactivity and minimum time before start state.
            _MinimumKeystrokes--;

            if (_SelectedFiddle == null && _PublishedFiddle == null && _MinimumDelay < Utility.GetMachineTime() && _MinimumKeystrokes <= 0)
            {
                // Transition to active state by selecting the next fiddle to publish.
                SelectNextFiddle();
            }

            if (_PublishedFiddle != null)
            {
                // Call fiddler to adjust our output.
                _PublishedFiddle.Implementation.ApplyOnKeyUp(_OutBuffer, key, this.ShiftPressed);
                if (_PublishedFiddle.Implementation.IsComplete)
                {
                    // Fiddle was applied, schedule the next one.
                    _PublishedFiddle.Implementation.AfterCompletion();
                    _PublishedFiddle = null;
                    _Ui.FiddlesMade = _Ui.FiddlesMade + 1;
                    SelectNextFiddle();
                }
            }
        }

        private void FiddleHandler(object arg)
        {
            if (_SelectedFiddle == null)
                return;

            // It's now time to publish the chosen fiddle.
            _SelectedFiddle.Implementation.OnPublish(_Rand);
            _PublishedFiddle = _SelectedFiddle;
            _SelectedFiddle = null;
            _NextFiddleEvents.Change(Timeout.Infinite, Timeout.Infinite);
            _Ui.CurrentState = UnitState.Applying;
        }
        
        private void InactivityTimerHandler(object arg)
        {
            // First off, deactivate the fiddle handler timer so nothing gets published.
            _NextFiddleEvents.Change(Timeout.Infinite, Timeout.Infinite);

            // Deactivate any fiddlers which may be (or may about to be) active.
            if (_PublishedFiddle != null)
            {
                _PublishedFiddle.Implementation.AfterCompletion();
                _PublishedFiddle = null;
            }
            if (_SelectedFiddle != null)
            {
                _SelectedFiddle.Implementation.AfterCompletion();
                _PublishedFiddle = null;
            }

            // The inactivity timer is now disabled until some keys are pressed.
            _InactivityTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _IsInactive = true;
            _Ui.CurrentState = UnitState.Inactive;
        }

        private void SelectNextFiddle()
        {
            // Start by choosing a fiddle to perform.
            var defs = _Config.Definitions;
            FiddleDefinition theChosenOne = null;
            if (defs.Length == 1)
                theChosenOne = defs[0];
            else
            {
                var rand = _Rand.Next();
                for (int i = 1; i < defs.Length; i++)
			    {
                    if (rand >= defs[i-1].Probability && rand < defs[i].Probability)
                    {
                        theChosenOne = defs[i-1];
                        break;
                    }
			    }
                if (theChosenOne == null)
                    theChosenOne = defs[defs.Length-1];
            }
            _SelectedFiddle = theChosenOne;
            _Ui.CurrentState = UnitState.Scheduled;

            // Now choose when to perform it.
            var millisecondRange = (int)((theChosenOne.MaxDelay.Ticks - theChosenOne.MinDelay.Ticks) / TimeSpan.TicksPerMillisecond);
            var millisecondOffset = (int)(theChosenOne.MinDelay.Ticks / TimeSpan.TicksPerMillisecond);
            var delayMilliseconds = _Rand.Next(millisecondRange) + millisecondOffset;
            if (_InDebugMode)
                delayMilliseconds = (int)(delayMilliseconds * _Config.DebugScaleingFactor);
            _NextFiddleEvents.Change(delayMilliseconds, Timeout.Infinite);
        }

        private void SetMinimumCounters()
        {
            if (!_InDebugMode)
            {
                _MinimumKeystrokes = _Config.MinKeystrokesToFirstFiddle;
                _MinimumDelay = Utility.GetMachineTime() + _Config.MinTimeToFirstFiddle;
            }
            else
            {
                _MinimumKeystrokes = (int)(_Config.MinKeystrokesToFirstFiddle * _Config.DebugScaleingFactor);
                _MinimumDelay = Utility.GetMachineTime() + new TimeSpan((long)(_Config.MinTimeToFirstFiddle.Ticks * _Config.DebugScaleingFactor));
            }
            _Ui.CurrentState = UnitState.Waiting;
        }

        private void SetInactivityTimeout()
        {
            var timeout = _Config.InactivityTimeout;
            if (_InDebugMode)
                timeout = new TimeSpan((long)(_Config.DebugScaleingFactor * _Config.InactivityTimeout.Ticks));
            _InactivityTimer.Change(timeout, timeout);
        }
    }
}
