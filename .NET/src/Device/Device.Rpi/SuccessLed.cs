﻿using System.Device.Gpio;
using Device.Rpi.Settings;
using Microsoft.Extensions.Logging;

namespace Device.Rpi;

public class SuccessLed : Led
{
    public SuccessLed(SuccessLedSettings ledSettings, GpioController gpioController, ILogger<SuccessLed> logger) : base(ledSettings, gpioController, logger)
    {
    }
}