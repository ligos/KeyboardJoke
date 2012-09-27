using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.USBHost;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class KeyboardInput
    {
        private USBH_Keyboard _Keyboard;
        private TimeSpan _KeyboardAttachedAt;           // To delay based on startup.
        private readonly DelayBuffer _OutBuffer;
        private readonly bool _InDebugMode;

        private readonly UserInterface _Ui;
        private readonly Random _Rand;
        private readonly FiddleConfig _Config;
        private readonly Timer _NextFiddleEvents;
        private FiddleDefinition _SelectedFiddle;       // This is set with the timer when the next fiddle definition is selected.
        private FiddleDefinition _PublishedFiddle;      // When the timer elapses this is set, it is checked on each key press to action the fiddle.
        private TimeSpan _LastKeyAction;                // Set to GetMachineTime() each key press to detect inactivity.
        private int _KeyPresses;

        public bool ShiftPressed { get; private set; }

        public KeyboardInput(UserInterface ui, KeyboardOutput output, FiddleConfig config, bool inDebugMode)
        {
            _Ui = ui;
            _OutBuffer = new DelayBuffer(output, ui);
            _Config = config;
            _NextFiddleEvents = new Timer(this.FiddleHandler, null, Timeout.Infinite, Timeout.Infinite);
            _Rand = new Random((int)(Utility.GetMachineTime().Ticks & 0x00000000ffffffff));
            _InDebugMode = inDebugMode;

        }

        public void BeginMonitorInputFrom(USBH_Device device)
        {
            _Keyboard = new USBH_Keyboard(device);
            _Keyboard.KeyUp += new USBH_KeyboardEventHandler(_HostKeyboard_KeyUp);
            _Keyboard.KeyDown += new USBH_KeyboardEventHandler(_HostKeyboard_KeyDown);
            _Keyboard.Disconnected += new USBH_KeyboardEventHandler(_Keyboard_Disconnected);
            _KeyboardAttachedAt = Utility.GetMachineTime();

            _LastKeyAction = _KeyboardAttachedAt;       // Initialise the timeout.
            SelectNextFiddle();                         // Select our first fiddle.
        }


        private void _Keyboard_Disconnected(USBH_Keyboard sender, USBH_KeyboardEventArgs args)
        {
            // Unhook event handlers and null our host keyboard.
            _Keyboard.KeyUp -= new USBH_KeyboardEventHandler(_HostKeyboard_KeyUp);
            _Keyboard.KeyDown -= new USBH_KeyboardEventHandler(_HostKeyboard_KeyDown);
            _Keyboard.Disconnected -= new USBH_KeyboardEventHandler(_Keyboard_Disconnected);
            _Keyboard = null;

            // Null any events.
            _NextFiddleEvents.Change(Timeout.Infinite, Timeout.Infinite);
            _PublishedFiddle = null;
            _SelectedFiddle = null;
        }

        private void _HostKeyboard_KeyDown(USBH_Keyboard sender, USBH_KeyboardEventArgs args)
        {
            // Ways to fiddle input:
            //  - Delay key strokes by an increasing amount for a few seconds, so it appears the keyboard is lagging.
            //  - Duplicate key strokes at random.
            //  - Insert additional phrases

            // Must have an inital delay period before anything happens so logins can happen OK.
            //  - A certain number of keystrokes and time must pass.


            _OutBuffer.KeyDown((byte)args.Key);
            _LastKeyAction = Utility.GetMachineTime();
            this.ShiftPressed = (_Keyboard.GetKeyState(USBH_Key.LeftShift) == USBH_KeyState.Down) || (_Keyboard.GetKeyState(USBH_Key.RightShift) == USBH_KeyState.Down);

            if (_PublishedFiddle != null)
            {
                // Call fiddler to adjust our output.
                _PublishedFiddle.Implementation.ApplyOnKeyDown(_OutBuffer, (byte)args.Key, this.ShiftPressed);
                if (_PublishedFiddle.Implementation.IsComplete)
                {
                    // Fiddle was applied, schedule the next one.
                    _PublishedFiddle.Implementation.AfterCompletion();
                    _PublishedFiddle = null;
                    _Ui.FiddlesMade = _Ui.FiddlesMade + 1;
                    _Ui.FiddleScheduled = false;

                    SelectNextFiddle();
                }
            }
        }

        private void _HostKeyboard_KeyUp(USBH_Keyboard sender, USBH_KeyboardEventArgs args)
        {
            _OutBuffer.KeyUp((byte)args.Key);
            _LastKeyAction = Utility.GetMachineTime();
            this.ShiftPressed = (_Keyboard.GetKeyState(USBH_Key.LeftShift) == USBH_KeyState.Down) || (_Keyboard.GetKeyState(USBH_Key.RightShift) == USBH_KeyState.Down);
            _KeyPresses++;      // Count key presses to allow for an initial delay.
            
            if (_PublishedFiddle != null)
            {
                // Call fiddler to adjust our output.
                _PublishedFiddle.Implementation.ApplyOnKeyUp(_OutBuffer, (byte)args.Key, this.ShiftPressed);
                if (_PublishedFiddle.Implementation.IsComplete)
                {
                    // Fiddle was applied, schedule the next one.
                    _PublishedFiddle.Implementation.AfterCompletion();
                    _PublishedFiddle = null;
                    _Ui.FiddlesMade = _Ui.FiddlesMade + 1;
                    _Ui.FiddleScheduled = false;

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
            _Ui.FiddleScheduled = true;
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

            // Now choose when to perform it.
            var millisecondRange = (int)((theChosenOne.MaxDelay.Ticks - theChosenOne.MinDelay.Ticks) / TimeSpan.TicksPerMillisecond);
            var millisecondOffset = (int)(theChosenOne.MinDelay.Ticks / TimeSpan.TicksPerMillisecond);
            var delayMilliseconds = _Rand.Next(millisecondRange) + millisecondOffset;
            if (_InDebugMode)
                delayMilliseconds = (int)(delayMilliseconds * _Config.DebugScaleingFactor);
            _NextFiddleEvents.Change(delayMilliseconds, Timeout.Infinite);
        }
    }
}
