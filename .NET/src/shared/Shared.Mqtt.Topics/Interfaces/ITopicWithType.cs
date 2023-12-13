namespace Shared.Mqtt.Topics.Interfaces;

public interface ITopicWithType
{
    public ITopicWithTypeAndDevice WithDevice(string deviceId);
}