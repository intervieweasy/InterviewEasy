namespace InterviewEasy.BuildingBlocks.EventBus.RabbitMQ;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; set; } = "localhost";
    public ushort Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";

    /// <summary>Prefix applied to every exchange and queue.</summary>
    public string Namespace { get; set; } = "intervieweasy";

    /// <summary>Retry attempts before moving to the dead-letter queue.</summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>Exponential backoff base interval (seconds).</summary>
    public int RetryBaseSeconds { get; set; } = 5;

    public string BuildConnectionString()
    {
        return $"amqp://{Username}:{Password}@{Host}:{Port}{VirtualHost}";
    }
}