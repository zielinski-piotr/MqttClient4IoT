using System.Device.I2c;

namespace Device.Rpi.Settings;

public class I2CConnectionSettings
{
    /// <summary>
    /// The bus ID the I2C device is connected to.
    /// </summary>
    public int BusId { get; set; }

    /// <summary>
    /// The bus address of the I2C device.
    /// </summary>
    public int DeviceAddress { get; set; }
    
    public I2cConnectionSettings ToI2cConnectionSettings() => new I2cConnectionSettings(BusId, DeviceAddress);
    
}