namespace InterviewEasy.BuildingBlocks.Observability.Tracing;

/// <summary>
/// Central registry of ActivitySource names used across the platform.
/// Every service emits spans under one of these sources.
/// </summary>
public static class ActivitySourceNames
{
    public const string Identity = "InterviewEasy.Identity";
    public const string Requirement = "InterviewEasy.Requirement";
    public const string Question = "InterviewEasy.Question";
    public const string Scheduling = "InterviewEasy.Scheduling";
    public const string Session = "InterviewEasy.Session";
    public const string Feedback = "InterviewEasy.Feedback";
    public const string Sandbox = "InterviewEasy.Sandbox";
    public const string Proctoring = "InterviewEasy.Proctoring";
    public const string Notification = "InterviewEasy.Notification";
    public const string Recording = "InterviewEasy.Recording";
    public const string Analytics = "InterviewEasy.Analytics";
    public const string Gateway = "InterviewEasy.Gateway";
    public const string Common = "InterviewEasy.Common";
    public const string EventBus = "InterviewEasy.EventBus";
}
