﻿namespace Shared.Mqtt.Topics.Interfaces;

public interface IEmptyTopic
{
    ITopicWithType WithType(TopicParts.Type topicTypePart);
}