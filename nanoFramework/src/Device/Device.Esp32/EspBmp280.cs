using Device.Abstractions;
using Iot.Device.Bmxx80;
using System;
using System.Device.I2c;
using System.Diagnostics;
using UnitsNet;

namespace Device.Esp32
{
    public class EspBmp280 : IDisposable, IPressureSensor, ITemperatureSensor
    {
        private readonly Bmp280 _i2CBmp280;

        public EspBmp280(I2cConnectionSettings i2cConnectionSettings)
        {
            var i2CDevice = I2cDevice.Create(i2cConnectionSettings);
            try
            {
                _i2CBmp280 = new Bmp280(i2CDevice);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error while creating instance of BMP280 sensor. Exception : '{e.Message}'");
                throw;
            }
        }

        public Pressure GetPressure()
        {
            _i2CBmp280.PressureSampling = Sampling.UltraHighResolution;
            var readResult = _i2CBmp280.Read();
            return readResult.Pressure;
        }

        public Temperature GetTemperature()
        {
            _i2CBmp280.TemperatureSampling = Sampling.LowPower;
            var readResult = _i2CBmp280.Read();
            return readResult.Temperature;
        }

        public void Dispose()
        {
            _i2CBmp280.Dispose();
        }
    }
}
