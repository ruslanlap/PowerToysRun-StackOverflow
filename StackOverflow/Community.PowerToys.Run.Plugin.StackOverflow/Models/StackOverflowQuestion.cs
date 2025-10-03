using System;

namespace Community.PowerToys.Run.Plugin.StackOverflow.Models
{
    /// <summary>
    /// Represents a single StackOverflow question with all metadata needed for display.
    /// </summary>
    public class StackOverflowQuestion
    {
        public int QuestionId { get; init; }
        public string Title { get; init; }
        public string Link { get; init; }
        public int Score { get; init; }
        public int AnswerCount { get; init; }
        public bool HasAcceptedAnswer { get; init; }
        public string[] Tags { get; init; } = Array.Empty<string>();
        public DateTime LastActivityDate { get; init; }
    }
}
