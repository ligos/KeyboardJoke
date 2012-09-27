using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class UserInterface
    {
        private const int ButtonPollInterval = 150;
        private const int PollsPerRefresh = 4;
        private readonly static TimeSpan BacklightDuration = TimeSpan.FromTicks(TimeSpan.TicksPerSecond * 15);

        private int _PollCount = 0;
        private LcdAndKeypad _Lcd;
        private Timer _Timer;
        private bool _DebuggerRunning;
        private bool _RequiresUpdate = false;
        private int _KeystrokesReceived = 0;
        private int _FiddlesMade = 0;
        private bool _FiddleScheduled = false;
        private TinyTimeSpan _KeyEchoDelay = TinyTimeSpan.Zero;
        private bool _LcdBacklightOn;
        private TimeSpan _TurnOffBacklightAfter = TimeSpan.Zero;

        public int KeystrokesReceived { get { return _KeystrokesReceived; } set { _KeystrokesReceived = value; _RequiresUpdate = true; } }
        public int FiddlesMade { get { return _FiddlesMade; } set { _FiddlesMade = value; _RequiresUpdate = true; } }
        public bool FiddleScheduled { get { return _FiddleScheduled; } set { _FiddleScheduled = value; _RequiresUpdate = true; } }
        public bool LcdBacklightOn { get { return _LcdBacklightOn; } set { _LcdBacklightOn = value; _RequiresUpdate = true; } }

        public UserInterface(LcdAndKeypad lcd, bool debuggerRunning)
        {
            _Lcd = lcd;
            _DebuggerRunning = debuggerRunning;
        }

        public void Start()
        {
            // The LCD does not have to be attached.
            if (_Lcd == null)
                return;
            if (_Timer != null)
                _Timer.Dispose();

            // Init the LCD with permanent text.
            _Lcd.Clear();
            _LcdBacklightOn = false;
            _Lcd.ShutBacklightOff();
            _Lcd.CursorHome();
            _Lcd.Print("Keyboard Joke");
            
            if (_DebuggerRunning)
            {
                // Backlight stays on permanently when debugger attached.
                _LcdBacklightOn = true;
                _Lcd.TurnBacklightOn();
                _TurnOffBacklightAfter = TimeSpan.MaxValue;

                // Show the debugger is on.
                _Lcd.SetCursorPosition(0, 14);
                _Lcd.Print('D');
            }
            _Lcd.CursorHome();

            // Now start the timer to keep it re-drawing and polling buttons;
            _Timer = new Timer(PollButtonPresses, null, ButtonPollInterval, ButtonPollInterval);
        }

        public void Stop()
        {
            if (_Timer != null)
            {
                _Timer.Change(Timeout.Infinite, Timeout.Infinite);
                _Timer.Dispose();
                _Timer = null;
            }
        }

        private void PollButtonPresses(object obj)
        {
            _PollCount++;
            var now = Utility.GetMachineTime();

            // Check the LCD button state.
            var key = _Lcd.GetKey();
            if (!_DebuggerRunning && key != LcdAndKeypad.Keys.None)
            {
                // If anything was pressed, turn the backlight on for a while.
                _RequiresUpdate = true;
                _TurnOffBacklightAfter = now.Add(BacklightDuration);
            }

            // This is also the hook into re-drawing the UI.
            if (_PollCount % PollsPerRefresh == 0)
            {
                // Backlight status.
                var newBacklightState = (now < _TurnOffBacklightAfter);
                if (newBacklightState != LcdBacklightOn)
                    LcdBacklightOn = newBacklightState;

                // UI redraw.
                _PollCount = 0;
                RedrawUi(obj);
            }
        }

        private void RedrawUi(object obj)
        {
            // If an exception has happened, don't display anything more.
            if (ExceptionService.Singleton.HasExceptionHappened)
                return;

            try
            {
                // 1234567890123456
                // Keyboard Joke DF     // Does not change; set in Start(). D = debugger available (keys are not echoed), X = fiiddle scheduled
                // 12345678  123456     // Count of keystrokes received, delay between receive and echo.


                // No updates: return quickly.
                if (!_RequiresUpdate)
                    return;

                // Backlight status.
                _Lcd.SetBacklightState(LcdBacklightOn);

                // Keystrokes received.
                _Lcd.SetCursorPosition(1, 0);
                _Lcd.Print(_KeystrokesReceived.ToString());

                // Fiddles made.
                _Lcd.SetCursorPosition(1, 10);
                _Lcd.Print(_FiddlesMade.ToString());

                // Fiddle scheduled.
                _Lcd.SetCursorPosition(0, 15);
                _Lcd.Print(_FiddleScheduled ? 'F' : ' ');
                
                _RequiresUpdate = false;
            }
            catch (Exception ex)
            {
                ExceptionService.Singleton.HandleException(ex);
            }
        }
    }
}
