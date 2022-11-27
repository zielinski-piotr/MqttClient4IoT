using UnitsNet;

namespace Device.Abstractions;

public interface IHumiditySensor
{
    public RelativeHumidity GetRelativeHumidity();
}