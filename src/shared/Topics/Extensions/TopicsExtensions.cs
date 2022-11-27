using System;

namespace Topics.Extensions;

public static class TopicsExtensions
{
    public static (TopicParts.Type, string, TopicParts.Application) ToTopicParts(this string topic)
    {
        var topicStringParts = topic.Split("/");

        if (topicStringParts.Length != TopicParts.TopicsPartsCount) throw new InvalidOperationException("Invalid number of topic parts");

        var topicTypePartString = topicStringParts[TopicParts.TopicTypePartIndex];

        if (!Enum.TryParse<TopicParts.Type>(topicTypePartString, true, out var topicType))
        {
            throw new InvalidOperationException($"Unknown topic Type part: '{topicTypePartString}'");
        }

        var topicDeviceIdPart = topicStringParts[TopicParts.TopicDeviceIdPartIndex];

        var topicApplicationPartString = topicStringParts[TopicParts.TopicApplicationIndex];

        if (!Enum.TryParse<TopicParts.Application>(topicApplicationPartString, true, out var topicApplication))
        {
            throw new InvalidOperationException($"Unknown topic Application part: '{topicApplicationPartString}'");
        }

        return (topicType, topicDeviceIdPart, topicApplication);
    }

    public static string Replace<T>(this string topic, string stringPart, T enumPart) where T : Enum
    {
        return topic.Replace(stringPart, enumPart.ToString());
    }
}