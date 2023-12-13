using System.Device.Gpio;
using Device.Abstractions;
using Device.Rpi.Settings;
using Microsoft.Extensions.Logging;

namespace Device.Rpi;

public class Led: ILed, IDisposable
{
    private readonly GpioController _gpioController;
    private readonly ILogger<Led> _logger;
    private readonly LedSettings _ledSettings;
    private readonly object _lock = new();
    private readonly GpioPin _pin;

    public Led(LedSettings ledSettings, GpioController gpioController, ILogger<Led> logger)
    {
        _gpioController = gpioController ?? throw new ArgumentNullException(nameof(gpioController));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ledSettings = ledSettings ?? throw new ArgumentNullException(nameof(ledSettings));
        _pin = _gpioController.OpenPin(_ledSettings.Pin, PinMode.Output);
    }
    
    public void On()
    {
        lock(_lock)
        {
            _pin.Write(OnValue);
            _logger.LogDebug("Set '{Value}' on Pin:{Pin}", OnValue, _pin.PinNumber);
        }
    }

    public void Off()
    {
        lock(_lock)
        {
            _pin.Write(OffValue);
            _logger.LogDebug("Set '{Value}' on Pin:{Pin}", OffValue, _pin.PinNumber);
        }
    }

    public void On(TimeSpan period)
    {
        Task.Run(() =>  
        {
            lock(_lock)
            {
                _pin.Write(OnValue);
                _logger.LogDebug("Set '{Value}' on Pin:{Pin}", OnValue, _pin.PinNumber);
                Thread.Sleep(period);
                _pin.Write(OffValue);
                _logger.LogDebug("Set '{Value}' on Pin:{Pin}", OffValue, _pin.PinNumber);
            }
        });
    }

    public void Off(TimeSpan period)
    {
        Task.Run(() =>  
        {
            lock(_lock)
            {
                _pin.Write(OffValue);
                _logger.LogDebug("Set '{Value}' on Pin:{Pin}", OffValue, _pin.PinNumber);
                Thread.Sleep(period);
                _pin.Write(OnValue);
                _logger.LogDebug("Set '{Value}' on Pin:{Pin}", OnValue, _pin.PinNumber);
            }
        });
    }

    private PinValue OnValue => _ledSettings.ResistorType == ResistorType.PullDown ? PinValue.High : PinValue.Low;
    private PinValue OffValue => _ledSettings.ResistorType == ResistorType.PullDown ? PinValue.Low : PinValue.High;

    public void Dispose()
    {
        _gpioController.ClosePin(_ledSettings.Pin);
    }
}