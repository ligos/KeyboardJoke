using System;
using Microsoft.SPOT;
using System.Threading;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class UserInterface
    {
        private const int ButtonPollInterval = 150;
        private const int PollsPerRefresh = 4;
        
        private int _PollCount = 0;
        private LcdAndKeypad _Lcd;
        private Timer _Timer;
        private bool _DebuggerRunning;
        private bool _RequiresUpdate = false;
        private int _KeystrokesReceived = 0;
        private GHIElectronics.NETMF.USBHost.USBH_Key _LastKeyPressed;
        private TinyTimeSpan _KeyEchoDelay = TinyTimeSpan.Zero;

        public int KeystrokesReceived { get { return _KeystrokesReceived; } set { _KeystrokesReceived = value; _RequiresUpdate = true; } }
        public GHIElectronics.NETMF.USBHost.USBH_Key LastKeyPressed { get { return _LastKeyPressed; } set { _LastKeyPressed = value; _RequiresUpdate = true; } }
        public TinyTimeSpan KeyEchoDelay { get { return _KeyEchoDelay; } set { _KeyEchoDelay = value; _RequiresUpdate = true; } }

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
            _Lcd.TurnBacklightOn();
            _Lcd.CursorHome();
            _Lcd.Print("Keyboard Joke");
            
            if (_DebuggerRunning)
            {
                _Lcd.SetCursorPosition(0, 15);
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
            
            // Check the LCD button state.
            var key = _Lcd.GetKey();
            if (key == LcdAndKeypad.Keys.Up)
                KeyEchoDelay = KeyEchoDelay.Add(10);
            else if (key == LcdAndKeypad.Keys.Down)
                KeyEchoDelay = KeyEchoDelay.Add(-10);
            if (KeyEchoDelay.Milliseconds < 0)
                KeyEchoDelay = TinyTimeSpan.Zero;


            // This is also the hook into re-drawing the UI.
            if (_PollCount % PollsPerRefresh == 0)
            {
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
                // Keyboard Joke  D     // Does not change; set in Start(). D = debugger available (keys are not echoed)
                // 1234        1000     // Count of keystrokes received, delay between receive and echo.

                // No updates: return quickly.
                if (!_RequiresUpdate)
                    return;

                // Keystrokes received.
                _Lcd.SetCursorPosition(1, 0);
                _Lcd.Print(_KeystrokesReceived.ToString());

                // Last keypress.
                if (_DebuggerRunning)
                {
                    _Lcd.SetCursorPosition(0, 0);
                    _Lcd.Print(' ', 3);
                    _Lcd.SetCursorPosition(0, 0);
                    _Lcd.Print(_LastKeyPressed.ToString());
                }

                // Delay between receiving and echoing keystrokes.
                _Lcd.SetCursorPosition(1, 12);
                _Lcd.Print(' ', 5);
                if (_KeyEchoDelay != TinyTimeSpan.Zero)
                {
                    _Lcd.SetCursorPosition(1, 12);
                    _Lcd.Print(_KeyEchoDelay.ToString(false));
                }
            }
            catch (Exception ex)
            {
                ExceptionService.Singleton.HandleException(ex);
            }
        }
    }
}
