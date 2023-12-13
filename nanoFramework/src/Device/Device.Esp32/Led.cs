using Device.Abstractions;
using System.Device.Gpio;
using System;
using Device.Esp32.Settings;
using System.Diagnostics;
using System.Threading;

namespace Device.Esp32
{
    public class Led : ILed, IDisposable
    {
        private readonly GpioController _gpioController;
        private readonly LedSettings _ledSettings;
        private readonly object _lock = new();
        private readonly GpioPin _pin;

        public Led(LedSettings ledSettings, GpioController gpioController)
        {
            _gpioController = gpioController ?? throw new ArgumentNullException(nameof(gpioController));
            _ledSettings = ledSettings ?? throw new ArgumentNullException(nameof(ledSettings));
            _pin = _gpioController.OpenPin(_ledSettings.Pin, PinMode.Output);
            _pin.Write(OffValue);
        }

        public void On()
        {
            lock (_lock)
            {
                _pin.Write(OnValue);
                Debug.WriteLine($"Set '{OnValue}' on Pin:{_pin.PinNumber}");
            }
        }

        public void Off()
        {
            lock (_lock)
            {
                _pin.Write(OffValue);
                Debug.WriteLine($"Set '{OffValue}' on Pin:{_pin.PinNumber}");
            }
        }

        public void On(TimeSpan period)
        {
            lock (_lock)
            {
                _pin.Write(OnValue);
                Debug.WriteLine($"Set '{OnValue}' on Pin:{_pin.PinNumber}");
                Thread.Sleep(period);
                _pin.Write(OffValue);
                Debug.WriteLine($"Set '{OffValue}' on Pin:{_pin.PinNumber}");
            }
        }

        public void Off(TimeSpan period)
        {
            lock (_lock)
            {
                _pin.Write(OffValue);
                Debug.WriteLine($"Set '{OffValue}' on Pin:{_pin.PinNumber}");
                Thread.Sleep(period);
                _pin.Write(OnValue);
                Debug.WriteLine($"Set '{OnValue}' on Pin:{_pin.PinNumber}");
            }
        }

        private PinValue OnValue => _ledSettings.ResistorType == ResistorType.PullDown ? PinValue.High : PinValue.Low;
        private PinValue OffValue => _ledSettings.ResistorType == ResistorType.PullDown ? PinValue.Low : PinValue.High;

        public void Dispose()
        {
            _pin.Write(OffValue);
            _gpioController.ClosePin(_ledSettings.Pin);
        }
    }
}
