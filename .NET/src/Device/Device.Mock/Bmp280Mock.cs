using System;
using Device.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace Device.Mock;

public class Bmp280Mock: IPressureSensor, ITemperatureSensor
{
    public Temperature GetTemperature()
    {
        var rnd = new Random();

        return new Temperature(rnd.NextDouble() * 20.0, TemperatureUnit.DegreeCelsius);
    }
        
    public Pressure GetPressure()
    {
        var rnd = new Random();

        return new Pressure(rnd.NextDouble() * 1000, PressureUnit.Hectopascal);
    }
}