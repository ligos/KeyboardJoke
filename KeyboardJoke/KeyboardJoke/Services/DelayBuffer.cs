using System;
using System.Threading;
using Microsoft.SPOT;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class DelayBuffer
    {
        private readonly KeyboardAndMouseOutput _KeyboardOut;
        private readonly FixedSizeQueue _Queue;
        private readonly Timer _Timer;
        private readonly UserInterface _UI;
        private bool _IsDelaying = false;

        public DelayBuffer(KeyboardAndMouseOutput keyboardOut, UserInterface ui)
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
        public void KeyUp(KeyboardKey key)
        {
            _Queue.Enqueue(QueuedEvent.CreateKeyUp(key));
            ActionEvent();
        }
        public void KeyDown(KeyboardKey key)
        {
            _Queue.Enqueue(QueuedEvent.CreateKeyDown(key));
            ActionEvent();
        }
        public void KeyPress(KeyboardKey key)
        {
            _Queue.Enqueue(QueuedEvent.CreateKeyPress(key));
            ActionEvent();
        }
        public void KeyPressWithModifier(ushort keyAndModifier)
        {
            if ((keyAndModifier & KeyboardTables.InvalidFlag) > 0)
                // This key is marked as invalid: ignore it.
                return;

            // TODO: other modifiers.
            if ((keyAndModifier & KeyboardTables.ShiftModifier) > 0)
                _Queue.Enqueue(QueuedEvent.CreateKeyDown(KeyboardKey.LeftShift));
            _Queue.Enqueue(QueuedEvent.CreateKeyPress((KeyboardKey)(keyAndModifier & 0x00ff)));
            if ((keyAndModifier & KeyboardTables.ShiftModifier) > 0)
                _Queue.Enqueue(QueuedEvent.CreateKeyUp(KeyboardKey.LeftShift));
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
