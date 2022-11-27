using UnitsNet;

namespace Device.Abstractions;

public interface IPressureSensor
{
    public Pressure GetPressure();
}