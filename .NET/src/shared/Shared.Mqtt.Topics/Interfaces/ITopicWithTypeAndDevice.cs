namespace Shared.Mqtt.Topics.Interfaces;

public interface ITopicWithTypeAndDevice
{
    public ITopicWithTypeAndDeviceAndApplication WithApplication(TopicParts.Application topicApplicationPart);
}