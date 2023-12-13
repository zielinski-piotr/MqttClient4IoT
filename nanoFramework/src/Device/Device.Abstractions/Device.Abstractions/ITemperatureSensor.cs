using UnitsNet;

namespace Device.Abstractions
{
    public interface ITemperatureSensor
    {
        public Temperature GetTemperature();
    }
}
