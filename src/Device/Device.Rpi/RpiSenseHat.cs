using System.Drawing;
using Device.Abstractions;
using Iot.Device.SenseHat;
using UnitsNet;

namespace Device.Rpi;

public class RpiSenseHat : ITemperatureSensor, IPressureSensor, IHumiditySensor, ILedMatrix
{
    private readonly SenseHat _senseHat;
    
    public RpiSenseHat(SenseHat senseHat)
    {
        _senseHat = senseHat;
    }

    public Temperature GetTemperature()
    {
        return _senseHat.Temperature;
    }

    public RelativeHumidity GetRelativeHumidity()
    {
        return _senseHat.Humidity;
    }

    public Pressure GetPressure()
    {
        return _senseHat.Pressure;
    }
    
    public void SetLedMatrix(ReadOnlySpan<Color> colors)
    {
        if (colors.Length > SenseHatLedMatrix.NumberOfPixels)
        {
            throw new ArgumentException("To many colors");
        }
        
        _senseHat.LedMatrix.Write(colors);
    }

    public void FillMatrix(Color color)
    {
        var colors = new Color[64];
        
        for (var i = 0; i < 64; i++)
        {
            colors[i] = color;
        }

        _senseHat.LedMatrix.Write(colors);
    }
}