using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Community.PowerToys.Run.Plugin.StackOverflow.Models
{
    /// <summary>
    /// Internal model for Stack Exchange API response deserialization.
    /// </summary>
    internal class ApiResponse
    {
        [JsonPropertyName("items")]
        public List<ApiQuestion> Items { get; set; } = new();

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("quota_remaining")]
        public int QuotaRemaining { get; set; }

        [JsonPropertyName("quota_max")]
        public int QuotaMax { get; set; }
    }

    internal class ApiQuestion
    {
        [JsonPropertyName("question_id")]
        public int QuestionId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("answer_count")]
        public int AnswerCount { get; set; }

        [JsonPropertyName("is_answered")]
        public bool IsAnswered { get; set; }

        [JsonPropertyName("has_accepted_answer")]
        public bool HasAcceptedAnswer { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; } = System.Array.Empty<string>();

        [JsonPropertyName("last_activity_date")]
        public long LastActivityDate { get; set; }
    }
}
