using System.Device.Gpio;
using Device.Abstractions;
using Device.Rpi.Settings;
using Microsoft.Extensions.Logging;

namespace Device.Rpi;

public class FailureLed : Led, IFailureLed
{
    public FailureLed(FailureLedSettings ledSettings, GpioController gpioController, ILogger<FailureLed> logger) : base(ledSettings, gpioController, logger)
    {
    }
}