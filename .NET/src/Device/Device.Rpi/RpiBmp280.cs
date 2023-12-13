using System.Device.I2c;
using Device.Abstractions;
using Device.Rpi.Settings;
using Iot.Device.Bmxx80;
using UnitsNet;

namespace Device.Rpi;

public class RpiBmp280 : IDisposable, IPressureSensor, ITemperatureSensor
{
    private readonly Bmp280 _i2CBmp280;
    
    public RpiBmp280(I2CConnectionSettings i2CConnectionSettings)
    {
        _ = i2CConnectionSettings ?? throw new ArgumentNullException(nameof(i2CConnectionSettings));
        
        var i2CDevice = I2cDevice.Create(i2CConnectionSettings.ToI2cConnectionSettings());
        _i2CBmp280 = new Bmp280(i2CDevice);
    }
    
    public Pressure GetPressure()
    {
        _i2CBmp280.PressureSampling = Sampling.UltraHighResolution;
        var readResult = _i2CBmp280.Read();
        return readResult.Pressure ?? Pressure.Zero ;
    }

    public Temperature GetTemperature()
    {
        _i2CBmp280.TemperatureSampling = Sampling.LowPower;
        var readResult = _i2CBmp280.Read();
        return readResult.Temperature ?? Temperature.Zero ;
    }
    
    public void Dispose()
    {
        _i2CBmp280.Dispose();
    }
}