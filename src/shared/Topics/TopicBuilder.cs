using Topics.Interfaces;
using Topics.Extensions;

namespace Topics;

public class TopicBuilder : ITopicWithType, ITopicWithTypeAndDevice, ITopicWithTypeAndDeviceAndApplication, IEmptyTopic
{
    private string Topic { get; set; } = TopicParts.TopicTemplate;

    private TopicBuilder()
    {
    }

    public static IEmptyTopic Create()
    {
        return new TopicBuilder();
    }

    public ITopicWithType WithType(TopicParts.Type topicTypePart)
    {
        Topic = Topic.Replace(TopicParts.TopicTypePart, topicTypePart);
        return this;
    }

    public ITopicWithTypeAndDevice WithDevice(string deviceId)
    {
        Topic = Topic.Replace(TopicParts.TopicDeviceIdPart, deviceId);
        return this;
    }

    public ITopicWithTypeAndDeviceAndApplication WithApplication(TopicParts.Application topicApplicationPart)
    {
        Topic = Topic.Replace(TopicParts.TopicApplication, topicApplicationPart);
        return this;
    }

    public string Build()
    {
        return Topic;
    }
}