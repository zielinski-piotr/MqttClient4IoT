using System.Device.Gpio;
using Device.Abstractions;
using Device.Rpi.Settings;
using Microsoft.Extensions.Logging;

namespace Device.Rpi;

public class SuccessLed : Led, ISuccessLed
{
    public SuccessLed(SuccessLedSettings ledSettings, GpioController gpioController, ILogger<SuccessLed> logger) : base(ledSettings, gpioController, logger)
    {
    }
}