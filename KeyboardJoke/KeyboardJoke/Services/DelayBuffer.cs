using System;
using System.Threading;
using Microsoft.SPOT;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class DelayBuffer
    {
        private readonly KeyboardOutput _KeyboardOut;
        private readonly FixedSizeQueue _Queue;
        private readonly Timer _Timer;
        private readonly UserInterface _UI;
        private bool _IsDelaying = false;

        public DelayBuffer(KeyboardOutput keyboardOut, UserInterface ui)
        {
            _KeyboardOut = keyboardOut;
            _Queue = new FixedSizeQueue(256);
            _Timer = new Timer(TimerTick, null, Timeout.Infinite, Timeout.Infinite);
            _UI = ui;
        }

        public void Delay(short milliseconds)
        {
            _Queue.Enqueue(QueuedEvent.CreateDelay(milliseconds));
            ActionEvent();
        }
        public void KeyUp(GHIElectronics.NETMF.USBClient.USBC_Key key)
        {
            _Queue.Enqueue(QueuedEvent.CreateKeyUp((byte)key));
            ActionEvent();
        }
        public void KeyDown(GHIElectronics.NETMF.USBClient.USBC_Key key)
        {
            _Queue.Enqueue(QueuedEvent.CreateKeyDown((byte)key));
            ActionEvent();
        }

        private void TimerTick(object arg)
        {
            // After a tick, disable the timer. It may be enabled again when we call ActionEvent().
            _Timer.Change(Timeout.Infinite, Timeout.Infinite);
            _IsDelaying = false;
            ActionEvent();
        }
        private void ActionEvent()
        {
            // If we're in a delay, wait for it to finish first.
            if (_IsDelaying)
                return;
            
            UInt32 e;
            if (!_Queue.TryDequeue(out e))
            {
                // Nothing left in the queue: return.
                return;
            }

            var type = e.GetEventType();
            if (type == EventType.None)
                // This shouldn't happen, but don't fail if it does.
                return;
            else if (type == EventType.KeyUp)
            {
                // Key up event: pass to output and dequeue next item.
                var key = e.GetKeyPressedAsClient();
                _KeyboardOut.KeyUp(key);
                _UI.LastKeyPressed = (GHIElectronics.NETMF.USBHost.USBH_Key)key;
                ActionEvent();
            }
            else if (type == EventType.KeyDown)
            {
                // Key down event: pass to output and dequeue next item.
                _KeyboardOut.KeyDown(e.GetKeyPressedAsClient());
                _UI.KeystrokesReceived = _UI.KeystrokesReceived + 1;
                ActionEvent();
            }
            else if (type == EventType.KeyPress)
            {
                // Key press event combines a key up and down.
                var key = e.GetKeyPressedAsClient();
                _KeyboardOut.KeyDown(key);
                _KeyboardOut.KeyUp(key);
                _UI.LastKeyPressed = (GHIElectronics.NETMF.USBHost.USBH_Key)key;
                _UI.KeystrokesReceived = _UI.KeystrokesReceived + 1;
                ActionEvent();
            }
            else if (type == EventType.Delay)
            {
                // Delay event: wait for specified time on timer thread before processing next item.
                // Alternative: just sleep this thread for the delay time (depends on what thread this is though).
                var milliseconds = e.GetDelay();
                _Timer.Change(milliseconds, milliseconds);
                _IsDelaying = true;
            }
            else
                throw new Exception("Unexpected state of event type: " + type);
        }


    }
}