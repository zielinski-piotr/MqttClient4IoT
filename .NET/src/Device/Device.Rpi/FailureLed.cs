using System.Device.Gpio;
using Device.Rpi.Settings;
using Microsoft.Extensions.Logging;

namespace Device.Rpi;

public class FailureLed : Led
{
    public FailureLed(FailureLedSettings ledSettings, GpioController gpioController, ILogger<FailureLed> logger) : base(ledSettings, gpioController, logger)
    {
    }
}