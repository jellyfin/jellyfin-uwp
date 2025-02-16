using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Gaming.Input;
using Windows.System;
using Windows.UI.Xaml;

namespace Jellyfin.Utils
{
    public sealed class GamepadManager
    {
        private readonly Dictionary<Gamepad, GamepadState> _gamepadStates = new Dictionary<Gamepad, GamepadState>();
        private const int ButtonPressCooldownMs = 250;
        private readonly DispatcherQueue _dispatcherQueue;
        private readonly DispatcherTimer _gamepadPollingTimer;

        public event Action OnBackPressed;

        public GamepadManager()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            Gamepad.GamepadAdded += Gamepad_Added;
            Gamepad.GamepadRemoved += Gamepad_Removed;

            _gamepadPollingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10),
            };
            _gamepadPollingTimer.Tick += GamepadPollingTimer_Tick;
            _gamepadPollingTimer.Start();
        }

        private void Gamepad_Added(object sender, Gamepad e)
        {
            if (!_gamepadStates.ContainsKey(e))
            {
                _gamepadStates[e] = new GamepadState();
            }
        }

        private void Gamepad_Removed(object sender, Gamepad e)
        {
            _gamepadStates.Remove(e);
        }

        private void GamepadPollingTimer_Tick(object sender, object e)
        {
            if (_dispatcherQueue.HasThreadAccess)
            {
                ProcessGamepadInput();
            }
            else
            {
                _dispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, ProcessGamepadInput);
            }
        }

        private void ProcessGamepadInput()
        {
            foreach (Gamepad gamepad in _gamepadStates.Keys)
            {
                GamepadState gamepadState = _gamepadStates[gamepad];
                GamepadReading reading = gamepad.GetCurrentReading();
                bool isBPressed = (reading.Buttons & GamepadButtons.B) == GamepadButtons.B;

                if (isBPressed && !gamepadState.WasBPressed && gamepadState.ButtonCooldownTimer.ElapsedMilliseconds >= ButtonPressCooldownMs) // press detected
                {
                    OnBackPressed?.Invoke();
                    gamepadState.WasBPressed = true;
                    gamepadState.ButtonCooldownTimer.Restart();
                }
                else if (!isBPressed)
                {
                    gamepadState.WasBPressed = false;
                }
            }
        }

        public void Dispose()
        {
            _gamepadPollingTimer.Stop();
            _gamepadPollingTimer.Tick -= GamepadPollingTimer_Tick;
            Gamepad.GamepadAdded -= Gamepad_Added;
            Gamepad.GamepadRemoved -= Gamepad_Removed;
        }
    }


    public sealed class GamepadState
    {
        public bool WasBPressed { get; set; } // track previous state of B button
        public Stopwatch ButtonCooldownTimer { get; set; } // tracks time since last B button press

        public GamepadState()
        {
            WasBPressed = false;
            ButtonCooldownTimer = new Stopwatch();
            ButtonCooldownTimer.Start();
        }
    }
}
