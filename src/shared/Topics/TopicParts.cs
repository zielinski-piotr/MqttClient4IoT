namespace Topics;

public static class TopicParts
{
    public const string TopicTypePart = "{DeviceType}";
    public const string TopicDeviceIdPart = "{DeviceId}";
    public const string TopicApplication = "{Application}";
    public const string TopicTemplate = $"{TopicTypePart}/{TopicDeviceIdPart}/{TopicApplication}";
    public const int TopicsPartsCount = 3;
    public const int TopicTypePartIndex = 0;
    public const int TopicDeviceIdPartIndex = 1;
    public const int TopicApplicationIndex = 2;
    
    public enum Type
    {
        Sensor
    }

    public enum Application
    {
        Temperature,
        LedMatrix,
        Pressure,
        Humidity
    }
}